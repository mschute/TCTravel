using System.Text.Json.Serialization;

namespace TCTravel.Models;

public class BookingLocation
{
    public int BookingId { get; set; }
    [JsonIgnore]
    public Booking? Booking { get; set; }
    
    public int LocationId { get; set; }
    [JsonIgnore]
    public Location? Location { get; set; }
    
}
