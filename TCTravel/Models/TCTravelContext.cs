using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TCTravel.Models;

public class TCTravelContext : IdentityDbContext<IdentityUser>
{
    public TCTravelContext(DbContextOptions<TCTravelContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ClientCompany> ClientCompanies { get; set; }
    public DbSet<Booking> Bookings { get; set; }
}