using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using meter_api.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace meter_api.Attributes
{
    public class AuthorisationHubFilter : IHubFilter
    {
        public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var httpContext = invocationContext.Context.GetHttpContext();
            if (httpContext == null)
                return await next(invocationContext);

            var jwtService = httpContext.RequestServices.GetRequiredService<IJwtService>();
            var agentTokenService = httpContext.RequestServices.GetRequiredService<IAgentTokenService>();
            var token = GetBearerToken(httpContext.Request.Headers.Authorization);

            if (string.IsNullOrEmpty(token) ||
                (!jwtService!.IsValidJwt(token) && !agentTokenService!.IsAgentTokenValid(token)))
            {
                throw new HubException("Unauthorized");
            }

            return await next(invocationContext);
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
