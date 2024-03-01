using System.Text.Json.Serialization;

namespace TCTravel.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly? Dob { get; set; }
    public string? Nationality { get; set; }
    
    [JsonIgnore]
    public ICollection<Booking>? Bookings { get; set; }
}