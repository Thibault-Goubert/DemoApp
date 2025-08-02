using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DemoApp.Functions.Functions.Middleware;

public class JwtMiddleware : IFunctionsWorkerMiddleware
{
    private readonly IConfiguration _config;

    public JwtMiddleware(IConfiguration config)
    {
        _config = config;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpRequest = await context.GetHttpRequestDataAsync();
        if (httpRequest is null || !httpRequest.Headers.TryGetValues("Authorization", out var authHeaders))
        {
            await next(context); // Aucun header = on passe la main
            return;
        }

        var bearerToken = authHeaders.FirstOrDefault(h => h.StartsWith("Bearer "));
        if (string.IsNullOrWhiteSpace(bearerToken))
        {
            await next(context);
            return;
        }

        var token = bearerToken["Bearer ".Length..].Trim();
        var secret = _config["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("JWT secret missing from configuration.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out _);

            // ✅ Stocker dans le contexte de la requête
            context.Items["User"] = claimsPrincipal;

        }
        catch (Exception)
        {
            // Token invalide => aucun utilisateur injecté
        }

        await next(context); // Toujours appeler next
    }
}
