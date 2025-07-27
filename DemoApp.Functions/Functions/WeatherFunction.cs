using DemoApp.Application.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace DemoApp.Functions.Functions;

public class WeatherFunction
{
    private readonly IWeatherService _weatherService;

    public WeatherFunction(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }
    [Function("GetWeather")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "weather")] HttpRequestData req)
    {
        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        var city = query["city"] ?? "Paris";

        var result = await _weatherService.GetWeatherAsync(city);

        var response = req.CreateResponse(HttpStatusCode.OK);

        await response.WriteAsJsonAsync(result);

        return response;
    }
}
