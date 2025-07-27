namespace DemoApp.Application.Services;

public interface IWeatherService
{
    Task<WeatherResult> GetWeatherAsync(string city);
}
