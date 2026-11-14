using DevPulse.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPulse.Data;

public class DevPulseDbContext : DbContext
{
    public DevPulseDbContext(
        DbContextOptions<DevPulseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>()
            .Property(p => p.StartDate)
            .HasColumnType("date");

        modelBuilder.Entity<Project>()
            .Property(p => p.DueDate)
            .HasColumnType("date");

        modelBuilder.Entity<Project>()
            .Property(p => p.EstimatedHours)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Project>()
            .Property(p => p.HoursSpent)
            .HasPrecision(10, 2);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}