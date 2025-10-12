using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using meter_api.Services;

namespace meter_api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresAuthorisationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();

            if (!authService.IsAuthorised(context.HttpContext))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
