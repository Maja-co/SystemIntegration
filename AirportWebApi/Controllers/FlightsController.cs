using AirportWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AirportWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController : ControllerBase {
    private static readonly List<FlightDeparture> _flights = new();


    // ------------- GET --------------------
    // Henter alle flyafgang
    [HttpGet]
    public ActionResult<IEnumerable<FlightDeparture>> GetFlights() {
        return Ok(_flights);
    }

    // ------------- POST --------------------
    // Opretter en flyafgang
    [HttpPost]
    public ActionResult<FlightDeparture> PostFlightDeparture(FlightDeparture flightDepature) {
        _flights.Add(flightDepature);
        return Ok(flightDepature);
    }

    // ------------- PUT --------------------
    // Opdaterer et fly
    [HttpPut("{flightNumber}")]
    public IActionResult PutFlight(string flightNumber, FlightDeparture updatedFlight) {
        var flight = _flights.Find(f => f.FlightNumber == flightNumber);
        if (flight == null) return NotFound("Flyet findes ikke");
        flight.Status = updatedFlight.Status;
        flight.DepartureTime = updatedFlight.DepartureTime;
        flight.Gate = updatedFlight.Gate;
        return NoContent();
    }
    
    // ------------ DELETE --------------------
    // Sletter en flyafgang
    [HttpDelete("{flightNumber}")]
    public IActionResult DeleteFlightDeparture(string flightNumber) {
        var flight = _flights.Find(f => f.FlightNumber == flightNumber);
        if (flight == null) return NotFound("Flyet findes ikke");
        _flights.Remove(flight);
        return NoContent();
    }
}