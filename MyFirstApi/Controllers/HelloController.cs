using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase {
    private readonly IGreetingService _greetingService;

    public HelloController(IGreetingService greetingService) {
        _greetingService = greetingService;
    }

    [HttpGet]
    public string Get() {
        return "Hej fra min første controller!";
    }

    [HttpGet("{name}")]
    public string Get(string name) {
        return _greetingService.CreateGreeting(name);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Person person) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        return Ok($"Modtaget person: {person.Name}, som er {person.Age} år gammel.");
    }
}