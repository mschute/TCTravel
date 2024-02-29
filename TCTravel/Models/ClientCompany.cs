namespace TCTravel.Models;

public class ClientCompany
{
    public int ClientCompanyId { get; set; }
    public string? ClientCompanyName { get; set; }
    public string? ClientCompanyAddress { get; set; }
    public string? ClientCompanyEmail { get; set; }
    public string? ClientCompanyPhone { get; set; }
    public string? ContactName { get; set; }
    
    public ICollection<Booking>? Bookings { get; set; }
}