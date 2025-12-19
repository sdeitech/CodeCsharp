using App.Application.Dto.AuthenticationModule;
using App.Application.Dto.SuperAdminAuthenticationModule.App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces.Repositories.SuperAdmin;
using App.Application.Interfaces.Services.SuperAdmin;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace App.Application.Service.SuperAdmin
{
    public class SuperAdminAuthenticationService(
        ISuperAdminAuthenticationRepository superAdminAuthenticationRepository,
        ISuperAdminTokenService superAdminTokenService,
        IConfiguration configuration) : ISuperAdminAuthenticationService
    {
        private readonly ISuperAdminAuthenticationRepository _superAdminAuthenticationRepository = superAdminAuthenticationRepository;
        private readonly ISuperAdminTokenService _superAdminTokenService = superAdminTokenService;

        public async Task<JsonModel> LoginAsync(SALoginDto loginDto)
        {
            var jsonModel = new JsonModel
            {
                Data = false,
                Message = StatusMessage.InternalServerError,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            try
            {
                var user = await _superAdminAuthenticationRepository.AuthenticateSuperAdminAsync(loginDto);

                if (user == null)
                {
                    jsonModel.Message = StatusMessage.InvalidUserOrPassword;
                    jsonModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return jsonModel;
                }

                // Generate new JWT token
                var tokenData = _superAdminTokenService.GenerateClaims(user);

                jsonModel.Data = tokenData;
                jsonModel.Message = StatusMessage.LoginSuccessfully;
                jsonModel.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                jsonModel.Message = StatusMessage.InternalServerError;
                jsonModel.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return jsonModel;
        }
    }
}
