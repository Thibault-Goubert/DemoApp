using DemoApp.Application.DTO;
using DemoApp.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DemoApp.Functions.Functions.Auth;

public class LoginFunction
{
    private readonly IAuthService _authService;
    private readonly ILogger _logger;

    public LoginFunction(IAuthService authService, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<LoginFunction>();
        _authService = authService;
    }

    [Function("Login")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "login")] 
        HttpRequestData req)
    {
        try
        {
            // 1. Vérifier le Content-Type
            if (!req.Headers.TryGetValues("Content-Type", out var contentType) ||
                !contentType.Any(c => c.Contains("application/json")))
            {
                var typeResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await typeResponse.WriteStringAsync("Content-Type must be application/json");
                return typeResponse;
            }

            // 2. Lire et valider le corps de la requête
            string rawBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(rawBody))
            {
                var emptyResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await emptyResponse.WriteStringAsync("Request body cannot be empty");
                return emptyResponse;
            }

            // 3. Journaliser le corps pour le débogage (à désactiver en production)
            // _logger.LogInformation($"Received body: {rawBody}");

            // 4. Désérialiser avec gestion d'erreur détaillée
            LoginRequest? data;
            try
            {
                data = JsonSerializer.Deserialize<LoginRequest>(
                    rawBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization failed");
                var jsonResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await jsonResponse.WriteStringAsync($"Invalid JSON format: {ex.Message}");
                return jsonResponse;
            }

            // 5. Validation des données
            if (data == null || string.IsNullOrEmpty(data.Username) || string.IsNullOrEmpty(data.Password))
            {
                var validationResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await validationResponse.WriteStringAsync("Username and password are required");
                return validationResponse;
            }

            // 6. Authentification
            string token = await _authService.GenerateTokenAsync(data.Username!, data.Password!);

            // 7. Réponse
            var response = req.CreateResponse(token != null ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            if (token != null)
            {
                
                await response.WriteAsJsonAsync(new
                {
                    token,
                    expiresIn = 3600,
                    tokenType = "Bearer"
                });
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in Login function");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("Internal server error");
            return errorResponse;
        }
    }
}