#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;


namespace Croissant_Rouge.Models;
public class MyContext : DbContext
{
    public MyContext(DbContextOptions options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Donation> Donations { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<Money> Moneys { get; set; }
}

