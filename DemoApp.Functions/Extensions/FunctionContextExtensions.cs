using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;

namespace DemoApp.Functions.Extensions;

public static class FunctionContextExtensions
{
    public static ClaimsPrincipal? GetCurrentUser(this FunctionContext context)
    {
        return context.Items.TryGetValue("User", out var userObj) && userObj is ClaimsPrincipal principal
            ? principal
            : null;
    }

    public static bool UserIsAuthenticated(this FunctionContext context)
    {
        var user = context.GetCurrentUser();
        return user?.Identity?.IsAuthenticated == true;
    }

    public static bool UserIsInRole(this FunctionContext context, string role)
    {
        var user = context.GetCurrentUser();
        return user?.IsInRole(role) == true;
    }

    public static string? GetUserName(this FunctionContext context)
    {
        return context.GetCurrentUser()?.Identity?.Name;
    }
}
