using DemoApp.Domain.Services;

namespace DemoApp.Application.Services;

public class GreetingService : IGreetingService
{
    public string GetGreeting(string name)
    {
        return $"Hello, {name}!";
    }
}
