namespace AirportAPI;

public class GateInfo {
    public int GateNumber { get; set; }
    public string FlightNumber { get; set; }

    public GateInfo(int gateNumber, string flightNumber) {
        GateNumber = gateNumber;
        FlightNumber = flightNumber;
    }
}