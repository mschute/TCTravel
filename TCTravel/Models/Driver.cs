using System.Text.Json.Serialization;

namespace TCTravel.Models;

public class Driver
{
    public int DriverId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? Dob { get; set; }
    public string? DrivingLicenseNo { get; set; }
    
    [JsonIgnore]
    public ICollection<Booking>? Bookings { get; set; }
}