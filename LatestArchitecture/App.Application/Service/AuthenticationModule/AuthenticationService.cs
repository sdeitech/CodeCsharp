using Amazon.Runtime.Internal.Util;
using App.Application.Dto;
using App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories;
using App.Application.Interfaces.Repositories.AuthenticationModule;
using App.Application.Interfaces.Repositories.LoginLogs;
using App.Application.Interfaces.Services;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Common.Constant;
using App.Common.Models;
using App.Common.Utility;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Enums.Common;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace App.Application.Service.AuthenticationModule
{
    public class AuthenticationService(IAuthenticationRepository authenticationRepository, IBruteForceProtectionService bruteForceProtectionService,
          IEmailService emailService,
          ITokenService tokenService,
          ILoginLogRepository loginLogRepository, IHttpContextAccessor httpContextAccessor, IDistributedCache cache,
          IConfiguration configuration,
                   IUnitOfWork unitOfWork) : IAuthenticationService
    {

        private readonly IAuthenticationRepository _authenticationRepository = authenticationRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBruteForceProtectionService _bruteForceProtectionService = bruteForceProtectionService;
        private readonly IEmailService _emailService = emailService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILoginLogRepository _loginLogRepository = loginLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IDistributedCache _cache = cache;
        private readonly IConfiguration _configuration = configuration;




        public async Task<JsonModel> GetOrgSettingsAsync(string organizationId)
        {
            var jsonModel = new JsonModel
            {
                Data = null,
                Message = StatusMessage.InternalServerError,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            var settings = await _authenticationRepository.GetOrgSettingsAsync(organizationId);

            if (settings != null && settings.Any())
            {
                var dto = new OrgSocialSettingsDto();

                foreach (var setting in settings)
                {
                    switch (setting.AgencySettingName.ToLower())
                    {
                        case "google":
                            dto.Google = setting.IsActive;
                            break;
                        case "microsoft":
                            dto.Microsoft = setting.IsActive;
                            break;
                        case "facebook":
                            dto.Facebook = setting.IsActive;
                            break;
                    }
                }

                jsonModel.Data = dto;
                jsonModel.Message = StatusMessage.RecordFetched;
                jsonModel.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                jsonModel.Message = StatusMessage.NoRecordFound;
                jsonModel.StatusCode = (int)HttpStatusCode.NotFound;
            }

            return jsonModel;
        }


        public async Task<JsonModel> AuthenticateAsync(LoginDto loginDto)
        {
            var jsonModel = new JsonModel
            {
                Data = false,
                Message = StatusMessage.InternalServerError,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            var clientIp = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);

            var result = await _authenticationRepository.AuthenticateAsync(loginDto);

            // Step 1: Check if user exists
            if (result == null)
            {
                jsonModel.Message = StatusMessage.UsernameDoesNotExist;
                jsonModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return jsonModel;
            }

            var decryptedPassword = CommonMethods.DecryptUserPassword(loginDto.Password);
            var bruteForceDto = new BruteForceProtectionDto
            {
                IpAddress = clientIp,
                UserName = loginDto.UserName,
                ClickEvent = "Login"
            };

            // Step 2: Block check
            if (result.BlockUntil.HasValue &&
                CommonMethods.TruncateToMinute(DateTime.UtcNow) < CommonMethods.TruncateToMinute(result.BlockUntil.Value))
            {
                var level = (OffenseLevel)result.OffenseLevel;
                jsonModel.Message = CommonMethods.GetEnumDescription(level);
                jsonModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return jsonModel;
            }

            // Step 3: Password check
            if (CommonMethods.VerifyPassword(decryptedPassword, result.Password))
            {
                if (!string.IsNullOrEmpty(result.RoleName))
                {
                    // Password expiry check
                    if (result.PasswordModifiedDate.HasValue)
                    {
                        var passwordDate = result.PasswordModifiedDate.Value.Date;
                        var daysSinceChange = (DateTime.UtcNow.Date - passwordDate).Days;
                        var daysRemaining = 90 - daysSinceChange;

                        if (daysRemaining <= 0)
                        {
                            jsonModel.Message = StatusMessage.PasswordExpired;
                            jsonModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                            jsonModel.Data = new { ForceReset = true };
                            return jsonModel;
                        }

                        if (daysRemaining <= 10)
                        {
                            jsonModel.Data = new
                            {
                                result.UserId,
                                result.Email,
                                result.OrganizationId,
                                result.Username,
                                result.RoleId,
                                result.RoleName,
                                result.PasswordModifiedDate,
                                result.AccessToken,
                                result.RefreshToken,
                                result.BlockUntil,
                                result.FailedAttempts,
                                result.OffenseLevel,
                                result.ThirdPartyLogin,

                                // extra flags
                                ShowPasswordExpiryWarning = true,
                                DaysRemaining = daysRemaining
                            };
                            jsonModel.Message = StatusMessage.LoginSuccessfully;
                            jsonModel.StatusCode = (int)HttpStatusCode.OK;
                            return jsonModel;
                        }
                    }

                    // Normal login success
                    await _bruteForceProtectionService.ResetAttemptsAsync(bruteForceDto);
                    await _authenticationRepository.UpdateThirdPartyLoginAsync(result.UserId, (int)ThirdPartyLoginType.None);

                    jsonModel.Data = new
                    {
                        result.UserId,
                        result.Email,
                        result.OrganizationId,
                        result.Username,
                        result.RoleId,
                        result.RoleName,
                        result.PasswordModifiedDate,
                        result.AccessToken,
                        result.RefreshToken,
                        result.BlockUntil,
                        result.FailedAttempts,
                        result.OffenseLevel,
                        result.ThirdPartyLogin
                    };
                    jsonModel.Message = StatusMessage.LoginSuccessfully;
                    jsonModel.StatusCode = (int)HttpStatusCode.OK;

                    _loginLogRepository.AddLoginLog(
                        null, null, loginDto.OrgnizationId, 1, clientIp!,
                        result.OrganizationId, StatusMessage.LoginSuccessfully, 1, bruteForceDto.ClickEvent
                    );

                    return jsonModel;
                }
            }


            // Step 4: Invalid password → existing brute-force logic
            var failedResult = await _bruteForceProtectionService.TrackLoginAttemptAsync(bruteForceDto);
            if (failedResult != null && failedResult.OffenseLevelReturn != 0)
            {
                var level = (OffenseLevel)failedResult.OffenseLevelReturn;
                jsonModel.Message = CommonMethods.GetEnumDescription(level);
                jsonModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return jsonModel;
            }

            jsonModel.Message = StatusMessage.InvalidUserOrPassword;
            jsonModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            return jsonModel;
        }




        public async Task<JsonModel> GoogleLoginAsync(Authenctication loginDto)
        {
            var jsonModel = new JsonModel
            {
                Data = false,
                Message = StatusMessage.InternalServerError,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            try
            {
                if (string.IsNullOrEmpty(loginDto.GoogleToken))
                {
                    jsonModel.Message = "Google token is required";
                    jsonModel.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonModel;
                }

                //  Validate token
                var googleClientId = _configuration["Authentication:Google:ClientId"];
                var payload = await GoogleJsonWebSignature.ValidateAsync(
                    loginDto.GoogleToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { googleClientId }
                    });

                if (payload == null || string.IsNullOrEmpty(payload.Email))
                {
                    jsonModel.Message = StatusMessage.InvalidGoogleToken;
                    jsonModel.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonModel;
                }

                //  Get user
                var userResponse = await _authenticationRepository.GetUserForSSOAsync(payload.Email, (int)ThirdPartyLoginType.Google);

                if (userResponse == null)
                {
                    jsonModel.Message = StatusMessage.UserNotFound;
                    jsonModel.StatusCode = StatusCodes.Status401Unauthorized;
                    return jsonModel;
                }

                if (userResponse is not null)
                {
                    //userResponse.ThirdPartyLogin = 1;
                    await _authenticationRepository.UpdateThirdPartyLoginAsync(userResponse.UserId, (int)ThirdPartyLoginType.Google);
                }

                //  Reset brute force attempts
                var clientIp = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
                await _bruteForceProtectionService.ResetAttemptsAsync(new BruteForceProtectionDto
                {
                    IpAddress = clientIp,
                    UserName = userResponse.Email,
                    ClickEvent = "GoogleLogin"
                });

                //  Log successful login
                _loginLogRepository.AddLoginLog(null, null, userResponse.OrganizationId, 1, clientIp, userResponse.UserId, StatusMessage.LoginSuccessfully, 1, "GoogleLogin");

                //  Generate JWT + Refresh Token
                var tokenData = _tokenService.GenerateClaims(userResponse);

                jsonModel.Data = tokenData;
                jsonModel.Message = StatusMessage.LoginSuccessfully;
                jsonModel.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception)
            {
                jsonModel.Message = StatusMessage.InternalServerError;
                jsonModel.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return jsonModel;
        }



        public async Task<JsonModel> FacebookLoginAsync(Authenctication loginDto)
        {
            var jsonModel = new JsonModel
            {
                Data = false,
                Message = StatusMessage.InternalServerError,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            try
            {
                if (string.IsNullOrEmpty(loginDto.FacebookToken))
                {
                    jsonModel.Message = "Facebook token is required";
                    jsonModel.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonModel;
                }

                // Validate Facebook token
                var appId = _configuration["Authentication:Facebook:AppId"];
                var appSecret = _configuration["Authentication:Facebook:AppSecret"];

                var fbValidationUrl =
                    $"https://graph.facebook.com/debug_token?input_token={loginDto.FacebookToken}&access_token={appId}|{appSecret}";

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(fbValidationUrl);

                if (!response.IsSuccessStatusCode)
                {
                    jsonModel.Message = StatusMessage.InvalidFacebookToken;
                    jsonModel.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonModel;
                }

                var validationResult = await response.Content.ReadFromJsonAsync<FacebookDebugResponse>();
                if (validationResult?.Data == null || !validationResult.Data.IsValid)
                {
                    jsonModel.Message = StatusMessage.InvalidFacebookToken;
                    jsonModel.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonModel;
                }

                // Get user info from Facebook
                var userInfoUrl =
                    $"https://graph.facebook.com/me?fields=id,email,first_name,last_name&access_token={loginDto.FacebookToken}";

                var userResponse = await httpClient.GetAsync(userInfoUrl);
                if (!userResponse.IsSuccessStatusCode)
                {
                    jsonModel.Message = StatusMessage.InvalidFacebookToken;
                    jsonModel.StatusCode = StatusCodes.Status400BadRequest;
                    return jsonModel;
                }

                var fbUser = await userResponse.Content.ReadFromJsonAsync<FacebookUserResponse>();

                if (string.IsNullOrEmpty(fbUser?.Email))
                {
                    jsonModel.Message = StatusMessage.EmailDoestNotExist;
                    jsonModel.StatusCode = StatusCodes.Status401Unauthorized;
                    return jsonModel;
                }

                // Check user in DB
                var user = await _authenticationRepository.GetUserByEmailAsync(fbUser.Email);
                if (user == null)
                {
                    jsonModel.Message = StatusMessage.EmailDoestNotExist;
                    jsonModel.StatusCode = StatusCodes.Status401Unauthorized;
                    return jsonModel;
                }

                // Update third-party login type
                await _authenticationRepository.UpdateThirdPartyLoginAsync(user.Id, (int)ThirdPartyLoginType.Facebook);

                // Reset brute force attempts
                var clientIp = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
                await _bruteForceProtectionService.ResetAttemptsAsync(new BruteForceProtectionDto
                {
                    IpAddress = clientIp,
                    UserName = user.Email,
                    ClickEvent = "FacebookLogin"
                });

                // Log success
                //_loginLogRepository.AddLoginLog(null, null, user.OrganizationId, 1, clientIp, user.Id, StatusMessage.LoginSuccessfully, 1, "FacebookLogin");

                //// Success response
                //jsonModel.Data = new
                //{
                //    user.Id,
                //    user.Email,
                //    user.FirstName,
                //    user.LastName,
                //    user.OrganizationId
                //};
                jsonModel.Message = StatusMessage.LoginSuccessfully;
                jsonModel.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception)
            {
                jsonModel.Message = StatusMessage.InternalServerError;
                jsonModel.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return jsonModel;
        }


        //public async Task<JsonModel> MicrosoftLoginAsync(Authenctication loginDto)
        //{
        //    var jsonModel = new JsonModel
        //    {
        //        Data = false,
        //        Message = StatusMessage.InternalServerError,
        //        StatusCode = StatusCodes.Status500InternalServerError
        //    };

        //    try
        //    {
        //        if (string.IsNullOrEmpty(loginDto.MicrosoftToken))
        //        {
        //            jsonModel.Message = "Microsoft token is required";
        //            jsonModel.StatusCode = StatusCodes.Status400BadRequest;
        //            return jsonModel;
        //        }

        //        // Validate Microsoft token
        //        var microsoftClientId = _configuration["Authentication:Microsoft:ClientId"];
        //        var tenantId = _configuration["Authentication:Microsoft:TenantId"];
        //        var authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";

        //        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        //        var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidIssuer = authority,
        //            ValidateAudience = true,
        //            ValidAudience = microsoftClientId,
        //            ValidateLifetime = true,
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKeys = await Microsoft.IdentityModel.Protocols.OpenIdConnect
        //                .OpenIdConnectConfigurationRetriever
        //                .GetAsync($"{authority}/.well-known/openid-configuration",
        //                          new Microsoft.IdentityModel.Protocols.ConfigurationManager<Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration>(
        //                              $"{authority}/.well-known/openid-configuration",
        //                              new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfigurationRetriever()))
        //                .ContinueWith(t => t.Result.SigningKeys)
        //        };

        //        tokenHandler.ValidateToken(loginDto.MicrosoftToken, validationParameters, out var validatedToken);
        //        var jwtToken = validatedToken as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;

        //        var email = jwtToken?.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value
        //                    ?? jwtToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

        //        if (string.IsNullOrEmpty(email))
        //        {
        //            jsonModel.Message = StatusMessage.InvalidMicrosoftToken;
        //            jsonModel.StatusCode = StatusCodes.Status400BadRequest;
        //            return jsonModel;
        //        }

        //        // Get user
        //        var user = await _authenticationRepository.GetUserByEmailAsync(email);
        //        if (user == null)
        //        {
        //            jsonModel.Message = StatusMessage.EmailDoestNotExist;
        //            jsonModel.StatusCode = StatusCodes.Status401Unauthorized;
        //            return jsonModel;
        //        }

        //        // Update login type
        //        await _authenticationRepository.UpdateThirdPartyLoginAsync(user.Id, (int)ThirdPartyLoginType.Microsoft);

        //        // Reset brute force attempts
        //        var clientIp = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
        //        await _bruteForceProtectionService.ResetAttemptsAsync(new BruteForceProtectionDto
        //        {
        //            IpAddress = clientIp,
        //            UserName = user.Email,
        //            ClickEvent = "MicrosoftLogin"
        //        });

        //        // Log successful login
        //        _loginLogRepository.AddLoginLog(null, null, user.OrganizationId, 1, clientIp, user.Id, StatusMessage.LoginSuccessfully, 1, "MicrosoftLogin");

        //        // Success response
        //        jsonModel.Data = new
        //        {
        //            user.Id,
        //            user.Email,
        //            user.FirstName,
        //            user.LastName,
        //            user.OrganizationId
        //        };
        //        jsonModel.Message = StatusMessage.LoginSuccessfully;
        //        jsonModel.StatusCode = StatusCodes.Status200OK;
        //    }
        //    catch (Exception)
        //    {
        //        jsonModel.Message = StatusMessage.InternalServerError;
        //        jsonModel.StatusCode = StatusCodes.Status500InternalServerError;
        //    }

        //    return jsonModel;
        //}






        public async Task<JsonModel> ForgotPasswordAsync(ResetUserPasswordDto resetUserPasswordDto)
        {
            JsonModel jsonModel = new JsonModel()
            {
                Data = false,
                Message = StatusMessage.InternalServerError,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            var clientIp = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
            resetUserPasswordDto.IpAddress = clientIp;

            resetUserPasswordDto.Token = Guid.NewGuid().ToString();
            var authenticationData = await _authenticationRepository.ForgotPasswordAsync(resetUserPasswordDto);

            if (authenticationData != null)
            {
                // Cache reset token + user info until OTP is verified
                var cacheKey = $"ForgotPwd:{authenticationData.UserId}";
                var cacheValue = JsonSerializer.Serialize(authenticationData);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                await _cache.SetStringAsync(cacheKey, cacheValue, cacheOptions);

                // Reuse your existing SendOtp flow
                var otpRequest = new EmailRequestDto
                {
                    UserId = authenticationData.UserId,
                    Email = authenticationData.Email,
                    Name = authenticationData.UserName,
                    //OrganizationId = authenticationData.OrganizationId
                };

                await _emailService.SendOtpEmailAsync(otpRequest);

                jsonModel.Message = StatusMessage.OTPSentToEmail;
                jsonModel.StatusCode = (int)HttpStatusCode.OK;
                jsonModel.Data = new { authenticationData.UserId };
            }
            else
            {
                jsonModel.Message = StatusMessage.EmailDoestNotExist;
                jsonModel.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            return jsonModel;
        }



        /// <summary>
        /// <Description>this method is use for reset password for user</Description>
        /// </summary>
        /// <param name = "userPassword" ></ param >
        /// < returns ></ returns >
        /// 

        public async Task<JsonModel> ResetPasswordAsync(ResetUserPasswordDto resetUserPasswordDto)
        {
            var decryptFrontendPassword = CommonMethods.DecryptUserPassword(resetUserPasswordDto.Password);
            resetUserPasswordDto.Password = CommonMethods.HashPassword(decryptFrontendPassword);

            resetUserPasswordDto.IpAddress = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);

            var result = await _authenticationRepository.ResetPasswordAsync(resetUserPasswordDto);

            if (result.Success && !string.IsNullOrEmpty(result.Email))
            {
                await _emailService.SendPasswordResetConfirmationEmailAsync(new PasswordResetEmailRequest
                {
                    Email = result.Email,
                    UserName = result.UserName,
                    OrganizationName = _emailService.GetSenderName()
                });

                return new JsonModel(true, StatusMessage.PasswordChangedSuccessfully, (int)HttpStatusCode.OK);
            }

            return new JsonModel(false, result.Message ?? StatusMessage.InternalServerError, (int)HttpStatusCode.InternalServerError);

        }




        public async Task<JsonModel> VerifyForgotPasswordOtpAsync(VerifyOtpRequest request)
        {
            if (request == null || request.UserId <= 0 || string.IsNullOrWhiteSpace(request.Otp))
            {
                return new JsonModel
                {
                    Message = StatusMessage.InvalidData,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            // Reuse existing VerifyOtp
            var user = await _authenticationRepository.VerifyOtpAsync(request.UserId, request.Otp);
            if (user == null)
            {
                return new JsonModel
                {
                    Message = StatusMessage.InvalidOTP,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            // Fetch ResetToken data from cache
            var cacheValue = await _cache.GetStringAsync($"ForgotPwd:{request.UserId}");
            if (string.IsNullOrEmpty(cacheValue))
            {
                return new JsonModel
                {
                    Message = StatusMessage.ForgotPasswordRequestNotFoundOrExpired,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            // Deserialize back into model
            var forgotData = JsonSerializer.Deserialize<ForgotPasswordResponseModel>(cacheValue);

            // Send reset email with token
            await _emailService.SendForgotPasswordEmailAsync(forgotData);

            // Clear cache entry
            await _cache.RemoveAsync($"ForgotPwd:{request.UserId}");

            return new JsonModel
            {
                Data = true,
                Message = StatusMessage.ResetPasswordEmailSentSuccessfully,
                StatusCode = (int)HttpStatusCode.OK
            };
        }




        public async Task<JsonModel> VerifyOtpAsync(VerifyOtpRequest request)
        {
            if (request == null || request.UserId <= 0 || string.IsNullOrWhiteSpace(request.Otp))
            {
                return new JsonModel
                {
                    Message = StatusMessage.InvalidData,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            var user = await _authenticationRepository.VerifyOtpAsync(request.UserId, request.Otp);

            if (user == null)
            {
                // OTP invalid or expired
                return new JsonModel
                {
                    Message = StatusMessage.InvalidOTP,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            // OTP valid → generate claims
            var tokenData = _tokenService.GenerateClaims(user);

            return new JsonModel
            {
                Data = tokenData,
                StatusCode = (int)HttpStatusCode.OK
            };
        }


        public async Task<JsonModel> GetUserByToken(string authToken, TokenModel tokenModel)
        {
            JsonModel jsonModel = new JsonModel()
            {
                Data = false,
                Message = StatusMessage.InternalServerError,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };


            var scheme = tokenModel.Request?.Request?.Scheme;
            var host = tokenModel.Request?.Request?.Host.Value;
            if (authToken != null)
            {
                var encryptData = _tokenService.GetDataFromToken(authToken);

                if (encryptData != null && encryptData.Claims != null)
                {
                    if (encryptData.Claims.Count > 0)
                    {
                        LoginDto loginDto = new LoginDto();
                        loginDto.UserName = encryptData.Claims[1].Value;
                        loginDto.OrgnizationId = Convert.ToInt32(encryptData.Claims[3].Value);

                        var result = await _authenticationRepository.AuthenticateAsync(loginDto);
                        result.AccessToken = authToken;


                        if (result != null)
                        {
                            jsonModel.Data = result;
                            jsonModel.StatusCode = (int)HttpStatusCode.OK;

                        }
                        else
                        {
                            jsonModel.Message = StatusMessage.InvalidToken;
                            jsonModel.StatusCode = (int)HttpStatusCode.Unauthorized;


                        }
                    }
                }


            }
            return jsonModel;
        }


    }
}

