using Microsoft.EntityFrameworkCore;
using Tracker.Web.Domain;

namespace Tracker.Web.Db;

public class AppDbContext : DbContext
{
    public DbSet<OrgStructElement> OrgStruct => Set<OrgStructElement>();
    public DbSet<Instruction> Instructions => Set<Instruction>();
 
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
        
        modelBuilder.Entity<Instruction>()   
            .Property(p => p.Name)   
            .HasColumnType("nvarchar")
            .HasMaxLength(255);
        
        modelBuilder.Entity<Instruction>()
            .HasOne<Instruction>()
            .WithMany(i => i.Children)
            .HasForeignKey(e => e.ParentId);
        
        modelBuilder.Entity<Instruction>()
            .HasOne(i => i.Creator)
            .WithMany()
            .HasForeignKey(e => e.CreatorId);
        
        modelBuilder.Entity<Instruction>()
            .HasOne(i => i.Executor)
            .WithMany()
            .HasForeignKey(e => e.ExecutorId);
    }
}