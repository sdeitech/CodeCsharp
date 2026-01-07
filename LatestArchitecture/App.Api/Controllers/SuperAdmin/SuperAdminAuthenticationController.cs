using App.Application.Dto.AuthenticationModule;
using App.Application.Dto.SuperAdminAuthenticationModule.App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces.Services.SuperAdmin;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.SuperAdmin
{
    [ApiController]
    [Route("/api/superadmin/[controller]")]
    public class SuperAdminAuthenticationController : ControllerBase
    {
        private readonly ISuperAdminAuthenticationService _superAdminAuthenticationService;

        public SuperAdminAuthenticationController(ISuperAdminAuthenticationService superAdminAuthenticationService)
        {
            _superAdminAuthenticationService = superAdminAuthenticationService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] SALoginDto loginDto)
        {
            var result = await _superAdminAuthenticationService.LoginAsync(loginDto);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status401Unauthorized => Unauthorized(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
    }
}
