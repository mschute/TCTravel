namespace TCTravel.Models;

public class Booking
{
    public int BookingId { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public Vehicle Vehicle { get; set; }
    public Driver Driver { get; set; }
    public Customer Customer { get; set; }
    public ClientCompany ClientCompany { get; set; }
}