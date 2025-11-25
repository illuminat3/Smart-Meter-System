using Microsoft.AspNetCore.SignalR;
using meter_api.Services;

namespace meter_api.Attributes;

public class AuthorisationHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var httpContext = invocationContext.Context.GetHttpContext();
        if (httpContext == null)
        {
            return await next(invocationContext);
        }

        var authService = httpContext.RequestServices.GetRequiredService<IAuthService>();

        return !authService.IsAuthorised(httpContext) ? throw new HubException("Unauthorized") : await next(invocationContext);
    }
}
