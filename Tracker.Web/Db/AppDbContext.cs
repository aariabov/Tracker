using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tracker.Web.Domain;

namespace Tracker.Web.Db;

public class AppDbContext : IdentityDbContext<User>
{
    public DbSet<OrgStructElement> OrgStruct => Set<OrgStructElement>();
    public DbSet<Instruction> Instructions => Set<Instruction>();
    
    public AppDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
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
            .HasOne(i => i.Parent)
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