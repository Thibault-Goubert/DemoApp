using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using DemoApp.Functions.Extensions;

namespace DemoApp.Functions.Functions.Secured;

public class SecureFunctionAdminData
{
    public SecureFunctionAdminData() {}

    [Function("SecureAdminData")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test")] HttpRequestData req,
        FunctionContext context)
    {
        if (!context.UserIsAuthenticated())
        {
            var unauth = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauth.WriteStringAsync("Unauthorized");
            return unauth;
        }

        if (!context.UserIsInRole("Admin"))
        {
            var forbidden = req.CreateResponse(HttpStatusCode.Forbidden);
            await forbidden.WriteStringAsync("Access forbidden: Admin role required");
            return forbidden;
        }

        var username = context.GetUserName() ?? "Unknown";

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync($"✅ Bienvenue Admin {username}, voici les données confidentielles !");
        return response;
    }
}
