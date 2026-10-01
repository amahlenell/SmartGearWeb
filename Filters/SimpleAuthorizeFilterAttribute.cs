using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartGearWeb.Filters
{
    // A lightweight authorization filter used to demonstrate this pipeline
    // stage ahead of full ASP.NET Core Identity (added in a later question).
    // It checks for ?admin=true on the request as a stand-in "logged in as
    // admin" signal, and blocks the request with 403 Forbidden if missing.
    public class SimpleAuthorizeFilterAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAdmin = context.HttpContext.Request.Query["admin"] == "true";

            if (!isAdmin)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }
    }
}
