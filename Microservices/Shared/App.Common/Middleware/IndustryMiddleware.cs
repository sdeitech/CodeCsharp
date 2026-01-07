using HES.SharedConfigs.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HES.SharedConfigs.Middleware
{
    public class IndustryMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, IIndustryConnectionService connectionService)
        {
            if (context.Request.Headers.TryGetValue("X-Industry", out var industry))
            {
                var connectionString = await connectionService.GetConnectionStringAsync(industry);
                context.Items["IndustryConnectionString"] = connectionString;
            }

            await _next(context);
        }
    }

}
