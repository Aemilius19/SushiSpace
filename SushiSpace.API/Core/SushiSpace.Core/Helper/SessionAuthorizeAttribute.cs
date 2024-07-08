using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SushiSpace.Core.Helper
{
    public class SessionAuthorizeAttribute : TypeFilterAttribute
    {
        public SessionAuthorizeAttribute() : base(typeof(SessionAuthorizeFilter))
        {
        }
    }

    public class SessionAuthorizeFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
