using App.Application.Dto;
using App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces.Services;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace App.Api.Controllers.AuthenticationModule
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthenticationController(IAuthenticationService authenticationService, IConfiguration configuration,
        IEmailService emailService
        ) : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IConfiguration _configuration = configuration;
        private readonly IEmailService _emailService = emailService;




        [HttpGet("org-settings/{orgId}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrgSettings(string orgId)
        {
            var result = await _authenticationService.GetOrgSettingsAsync(orgId);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }















        /// <summary>
        /// Authenticates a user with the provided login credentials.
        /// </summary>
        /// <param name="loginDto">The login credentials (username/email and password).</param>
        /// <returns>
        /// Returns a <see cref="JsonModel"/> containing authentication details if successful, 
        /// or an error message if authentication fails.
        /// </returns>
        /// <response code="200">Returns the authenticated user info and token.</response>
        /// <response code="401">Returned when the credentials are invalid.</response>
        /// <response code="500">Returned when an internal server error occurs.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
            {
            var result = await _authenticationService.AuthenticateAsync(loginDto);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status401Unauthorized => Unauthorized(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        [HttpPost("login/google")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleLogin([FromBody] Authenctication authenctication)
        {
            var result = await _authenticationService.GoogleLoginAsync(authenctication);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status401Unauthorized => Unauthorized(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }



        [HttpPost("login/facebook")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FacebookLogin([FromBody] Authenctication authenctication)
        {
            var result = await _authenticationService.FacebookLoginAsync(authenctication);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status401Unauthorized => Unauthorized(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }





        //[HttpPost("login/microsoft")]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> MicrosoftLogin([FromBody] Authenctication authenctication)
        //{
        //    var result = await _authenticationService.MicrosoftLoginAsync(authenctication);

        //    return result.StatusCode switch
        //    {
        //        StatusCodes.Status200OK => Ok(result),
        //        StatusCodes.Status400BadRequest => BadRequest(result),
        //        StatusCodes.Status401Unauthorized => Unauthorized(result),
        //        _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        //    };
        //}







        /// <summary>
        /// Initiates the password reset process for a user based on provided information.
        /// </summary>
        /// <param name="resetUserPasswordDto">
        /// The DTO containing necessary user identification information, 
        /// such as email or username, to request a password reset.
        /// </param>
        /// <returns>
        /// A <see cref="JsonModel"/> indicating whether the password reset request was successful or failed.
        /// </returns>
        /// /// <response code="200">Password reset instructions were sent successfully.</response>
        /// <response code="401">The provided information is invalid or the user is unauthorized.</response>
        /// <response code="500">An internal server error occurred while processing the request.</response>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetUserPasswordDto resetUserPasswordDto)
        {
            var result = await _authenticationService.ForgotPasswordAsync(resetUserPasswordDto);
            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status401Unauthorized => Unauthorized(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Resets the user's password using the provided reset token and new password.
        /// </summary>
        /// <param name="resetUserPasswordDto">
        /// The DTO containing the reset token, new password, and other required information.
        /// </param>
        /// <returns>
        /// A <see cref="JsonModel"/> indicating whether the password reset was successful or failed.
        /// </returns>
        /// <response code="200">Password was successfully reset.</response>
        /// <response code="400">The reset token is invalid or the request is malformed.</response>
        /// <response code="500">An internal server error occurred while resetting the password.</response>
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetUserPasswordDto resetUserPasswordDto)
        {
            var result = await _authenticationService.ResetPasswordAsync(resetUserPasswordDto);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }


        /// <summary>
        /// Sends a One-Time Password (OTP) email to the specified email address.
        /// </summary>
        /// <param name="request">The email request containing the recipient's email address.</param>
        /// <returns>
        /// A <see cref="JsonModel"/> indicating whether the OTP email was sent successfully or if an error occurred.
        /// </returns>
        /// <response code="200">The OTP email was sent successfully.</response>
        /// <response code="400">The email request is invalid.</response>
        /// <response code="500">An internal server error occurred while sending the OTP email.</response>
        [HttpPost("send-otp")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendOtp([FromBody] EmailRequestDto request)
        {
            var result = await _emailService.SendOtpEmailAsync(request);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }


        /// <summary>
        /// Regenerates and sends a new One-Time Password (OTP) email to the specified email address.
        /// </summary>
        /// <param name="request">The email request containing the recipient's email address.</param>
        /// <returns>
        /// A <see cref="JsonModel"/> indicating whether the OTP was regenerated and sent successfully or if an error occurred.
        /// </returns>
        /// <response code="200">The new OTP email was sent successfully.</response>
        /// <response code="400">The email request is invalid.</response>
        /// <response code="500">An internal server error occurred while sending the OTP email.</response>
        [HttpPost("regenerate-otp")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegenerateOtp([FromBody] EmailRequestDto request)
        {
            var result = await _emailService.SendOtpEmailAsync(request);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }


        /// <summary>
        /// Verifies the One-Time Password for the Forgot Password (OTP) provided by the user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// /// A <see cref="JsonModel"/> indicating whether the OTP verification was successful or failed.
        /// </returns>
        /// <response code="200">OTP verified successfully.</response>
        /// <response code="400">Invalid OTP or bad request.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpPost("verify-forgot-otp")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyForgotPasswordOtp([FromBody] VerifyOtpRequest request)
        {
            var result = await _authenticationService.VerifyForgotPasswordOtpAsync(request);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }





        /// <summary>
        /// Verifies the One-Time Password (OTP) provided by the user.
        /// </summary>
        /// <param name="request">The OTP verification request containing the OTP and associated information.</param>
        /// <returns>
        /// A <see cref="JsonModel"/> indicating whether the OTP verification was successful or failed.
        /// </returns>
        /// <response code="200">OTP verified successfully.</response>
        /// <response code="400">Invalid OTP or bad request.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="500">Internal server error occurred.</response>
       // [Authorize(Roles = "admin")]
        [HttpPost("verify-otp")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = await _authenticationService.VerifyOtpAsync(request);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status401Unauthorized => Unauthorized(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }


        /// <summary>
        /// Retrieves user information based on the provided authorization token.
        /// </summary>
        /// <returns>
        /// A <see cref="JsonModel"/> containing user information if the token is valid.
        /// </returns>
        /// <response code="200">User information retrieved successfully.</response>
        /// <response code="401">Unauthorized - token is missing or invalid.</response>
        /// <response code="500">Internal server error occurred.</response>

        [Authorize]
        [HttpGet("user/refresh-token")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserByToken()
        {
            StringValues authorizationToken;
            TokenModel tokenModel = new TokenModel();
            tokenModel.Request = HttpContext;
            var authHeader = HttpContext.Request.Headers.TryGetValue("Authorization", out authorizationToken);
            var authToken = authorizationToken.ToString().Replace("Bearer", "").Trim();
            var response = await _authenticationService.GetUserByToken(authToken, tokenModel);
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status401Unauthorized => Unauthorized(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };

        }

    }
}
