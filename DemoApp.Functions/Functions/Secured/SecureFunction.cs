using System.Net;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DemoApp.Functions.Functions.Secured;

public class SecureDataFunction
{
    public SecureDataFunction() { }

    [Function("SecureData")]
    public async Task<HttpResponseData> RunSecure(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "secure-data")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("SecureData");

        if (!executionContext.Items.TryGetValue("User", out var userObj) || userObj is not ClaimsPrincipal user)
        {
            var unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorized.WriteStringAsync("Unauthorized");
            return unauthorized;
        }

        var username = user.Identity?.Name ?? "Unknown";
        var roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync($"Bienvenue {username} ✅ (Rôles: {string.Join(", ", roles)})");

        return response;
    }
}
