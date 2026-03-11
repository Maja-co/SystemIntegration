namespace AirportWebApi.Models;

public class FlightDeparture {
    //String FlightNumber, String Destination, datetime Departure Time, string Gate og enum Status?
    public string FlightNumber { get; set; }
    public string Destination { get; set; }
    public DateTime DepartureTime { get; set; }
    public string Gate { get; set; }
    public FlightStatus Status { get; set; } 
}