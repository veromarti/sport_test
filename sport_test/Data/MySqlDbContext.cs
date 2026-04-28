using Microsoft.EntityFrameworkCore;
using sport_test.Models;

namespace sport_test.Data;

public class MySqlDbContext : DbContext
{
    public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options)
    {
    }

    public DbSet<User> users { get; set; }
    public DbSet<SportSpace> sport_spaces { get; set; }
    public DbSet<Reservation> reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Document).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<SportSpace>(entity =>
        {
            entity.HasIndex(s => s.Name).IsUnique();
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.SportSpace)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SportSpaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
