namespace TCTravel.Models;

public class Location
{
    public int LocationId { get; set; }
    public string LocationName { get; set; }
    public string LocationAddress { get; set; }
    
    public virtual ICollection<Booking> Bookings { get; set; }
}