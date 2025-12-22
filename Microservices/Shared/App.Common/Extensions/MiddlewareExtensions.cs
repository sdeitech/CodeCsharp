using HES.SharedConfigs.Middleware;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HES.SharedConfigs.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseIndustryContext(this IApplicationBuilder app)
        {
            return app.UseMiddleware<IndustryMiddleware>();
        }
    }

}
