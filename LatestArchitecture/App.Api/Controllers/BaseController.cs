using App.Common;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    public class BaseController : ControllerBase
    {

        [NonAction]
        public TokenModel GetToken(HttpContext httpContext)
        {
            return CommonMethods.GetTokenDataModel(httpContext);
        }

    }

}
