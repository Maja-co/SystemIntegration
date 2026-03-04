namespace MyFirstApi.Services;

public class GreetingService : IGreetingService {
    public string CreateGreeting(string name) {
        return $"Hej fra service, {name}!";
    }
}