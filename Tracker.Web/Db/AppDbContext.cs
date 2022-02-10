using Microsoft.EntityFrameworkCore;
using Tracker.Web.Domain;

namespace Tracker.Web.Db;

public class AppDbContext : DbContext
{
    public DbSet<OrgStructElement> OrgStruct => Set<OrgStructElement>();
 
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=tracker.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrgStructElement>()   
            .Property(p => p.Name)   
            .HasColumnType("nvarchar")
            .HasMaxLength(255);

        modelBuilder.Entity<OrgStructElement>()
            .HasOne<OrgStructElement>()
            .WithMany()
            .HasForeignKey(e => e.ParentId);
    }
}