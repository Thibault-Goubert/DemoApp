using DemoApp.Application.Services;
using System.Text.RegularExpressions;

namespace DemoApp.Infrastructure.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherResult> GetWeatherAsync(string city)
    {
        var raw = await _httpClient.GetStringAsync($"https://wttr.in/{city}?format=3");

        // Exemple : "Paris: 🌦 +18°C"
        var match = Regex.Match(raw, @"^(.*?):\s+(\S+)\s+([\+\-]?\d+°C)$");

        if (!match.Success)
            return new WeatherResult { Raw = raw };

        return new WeatherResult
        {
            City = match.Groups[1].Value.Trim(),
            Condition = match.Groups[2].Value.Trim(),
            Temperature = match.Groups[3].Value.Trim(),
            Raw = raw
        };
    }
}