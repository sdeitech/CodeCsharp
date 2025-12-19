using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Api.Middleware
{
    public class SuperAdminJwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _jwtSecret;

        public SuperAdminJwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _jwtSecret = configuration["SuperAdminJwtSettings:Key"]
                         ?? configuration["JwtSettings:Key"]
                         ?? throw new ArgumentNullException("JwtSettings:Key", "JWT secret key is missing from configuration.");
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token) && CanReadToken(token))
            {
                AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private bool CanReadToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.CanReadToken(token);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Check if user has SuperAdmin role
                var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                if (roleClaim != null && roleClaim.Value == "SuperAdmin")
                {
                    context.User = principal;
                }
                else
                {
                    // Not a super admin, don't attach to context
                    context.User = new ClaimsPrincipal(new ClaimsIdentity());
                }
            }
            catch (Exception ex)
            {
                // Log the error properly
                Console.WriteLine($"SuperAdmin JWT Validation failed: {ex.Message}");
                // Optionally, set context.User to an empty principal to prevent unauthorized access
                context.User = new ClaimsPrincipal(new ClaimsIdentity());
            }
        }
    }
}
