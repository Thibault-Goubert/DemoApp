

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DemoApp.Functions.Functions.Auth;

public class SecureDataFunction
{
    private readonly IConfiguration _config;
    private readonly ILogger _logger;

    public SecureDataFunction(IConfiguration config, ILoggerFactory loggerFactory)
    {
        _config = config;
        _logger = loggerFactory.CreateLogger<SecureDataFunction>();
    }

    [Function("SecureData")]
    public async Task<HttpResponseData> RunSecure(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "secure-data")] HttpRequestData req)
    {
        _logger.LogInformation("SecureData function called");

        if (!req.Headers.TryGetValues("Authorization", out var values))
            return req.CreateResponse(HttpStatusCode.Unauthorized);

        var authHeader = values.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer "))
            return req.CreateResponse(HttpStatusCode.Unauthorized);

        var token = authHeader["Bearer ".Length..].Trim();
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured"));

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;
            var username = jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value;

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Bonjour {username}, tu es bien authentifié ✅");

            _logger.LogInformation("Token valid, returning success");

            return response;
        }
       catch (SecurityTokenException ex)
        {
            _logger.LogWarning($"Token validation failed: {ex.Message}");
            return req.CreateResponse(HttpStatusCode.Unauthorized);
        }
    }
}
