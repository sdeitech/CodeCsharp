using App.Application.Interfaces.Services.AuthenticationModule;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace App.Application.Service.AuthenticationModule
{
    public class CurrentUserClaimService(IHttpContextAccessor httpContextAccessor) : ICurrentUserClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public int? UserId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst("UserID")
                    ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

                return int.TryParse(claim?.Value, out var id) ? id : null;
            }
        }

        public string? UserName =>
            _httpContextAccessor.HttpContext?.User?
                .FindFirst("Username")?.Value
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        public string? Role =>
            _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Role)?.Value;

        public int? OrganizationId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst("OrganizationId");

                return int.TryParse(claim?.Value, out var id) ? id : null;
            }
        }

    }
}



