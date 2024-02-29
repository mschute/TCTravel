namespace TCTravel.Models;

public class Driver
{
    public int DriverId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? Dob { get; set; }
    public string? DrivingLicenseNo { get; set; }
    
    public ICollection<Booking> Bookings { get; set; }
}