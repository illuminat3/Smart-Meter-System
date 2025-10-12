using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using meter_api.Services;

namespace meter_api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresAuthorisationAttribute : Attribute, IAsyncAuthorizationFilter, IAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            var agentTokenService = context.HttpContext.RequestServices.GetService<IAgentTokenService>();

            var token = GetBearerToken(context.HttpContext.Request.Headers.Authorization);

            if (string.IsNullOrEmpty(token) ||
                (!jwtService!.IsValidJwt(token) && !agentTokenService!.IsAgentTokenValid(token)))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await Task.CompletedTask;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            var agentTokenService = context.HttpContext.RequestServices.GetService<IAgentTokenService>();

            var token = GetBearerToken(context.HttpContext.Request.Headers.Authorization);

            if (string.IsNullOrEmpty(token) ||
                (!jwtService!.IsValidJwt(token) && !agentTokenService!.IsAgentTokenValid(token)))
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private static string? GetBearerToken(string? authHeader)
        {
            if (string.IsNullOrWhiteSpace(authHeader))
                return null;

            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return null;

            return authHeader["Bearer ".Length..].Trim();
        }
    }
}
