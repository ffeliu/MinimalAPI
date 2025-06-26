using Microsoft.EntityFrameworkCore;
using minimalapi.domain.entity;

namespace minimalapi.domain.db;

public class AppDbContext : DbContext
{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }

    private readonly IConfiguration _configuration;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Name = "Admin",
                    Email = "admin@teste.com",
                    Password = "admin123",
                    CreatedAt = new DateTime(2025, 6, 25, 16, 25, 17),
                    UpdatedAt = new DateTime(2025, 6, 25, 16, 25, 17)
                }
            );
    }
}
    
    