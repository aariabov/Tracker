using Microsoft.EntityFrameworkCore;
using Tracker.Audit.Db.Models;

namespace Tracker.Audit.Db;

public class AuditDbContext : DbContext
{
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>();
    }
}
