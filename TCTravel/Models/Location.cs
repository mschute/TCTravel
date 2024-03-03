using System.Text.Json.Serialization;

namespace TCTravel.Models;

public class Location
{
    public int LocationId { get; set; }
    public string LocationName { get; set; }
    public string LocationAddress { get; set; }
    
    public ICollection<BookingLocation>? BookingLocations { get; set; }
}