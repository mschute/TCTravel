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

    public DbSet<BookingLocation> BookingLocations { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<BookingLocation>()
            .HasKey(bl => new { bl.BookingId, bl.LocationId });
        modelBuilder.Entity<BookingLocation>()
            .HasOne(bl => bl.Booking)
            .WithMany(l => l.BookingLocations)
            .HasForeignKey(b => b.BookingId);
        modelBuilder.Entity<BookingLocation>()
            .HasOne(bl => bl.Location)
            .WithMany(l => l.BookingLocations)
            .HasForeignKey(b => b.LocationId);
    }
}