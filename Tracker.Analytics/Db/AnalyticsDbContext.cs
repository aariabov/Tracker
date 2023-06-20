using Microsoft.EntityFrameworkCore;
using Tracker.Analytics.Db.Models;

namespace Tracker.Analytics.Db;

public class AnalyticsDbContext : DbContext
{
    public DbSet<Instruction> Instructions => Set<Instruction>();
    public DbSet<User> Users => Set<User>();

    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Instruction>(b =>
        {
            b.Property(p => p.Name).HasMaxLength(255);

            b.HasOne(i => i.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);

            b.HasOne(i => i.Executor)
                .WithMany()
                .HasForeignKey(e => e.ExecutorId);
        });
    }
}
