using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tracker.Db.Models;

namespace Tracker.Db;

public class AppDbContext : IdentityDbContext<User, Role, string>
{
    public DbSet<Instruction> Instructions => Set<Instruction>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public AppDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("asp_net_user_tokens");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("asp_net_user_logins");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("asp_net_user_claims");
        modelBuilder.Entity<Role>().ToTable("asp_net_roles").HasIndex(r => r.NormalizedName).HasDatabaseName("role_name_index");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("asp_net_user_roles");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("asp_net_role_claims");

        modelBuilder.Entity<Instruction>(b =>
        {
            b.Property(p => p.Name).HasMaxLength(255);
            
            b.HasOne(i => i.Parent)
                .WithMany(i => i.Children)
                .HasForeignKey(e => e.ParentId);
            
            b.HasOne(i => i.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            
            b.HasOne(i => i.Executor)
                .WithMany()
                .HasForeignKey(e => e.ExecutorId);
        });
        
        modelBuilder.Entity<User>(b =>
        {
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.BossId);
            
            b.HasMany(e => e.Roles)
                .WithMany(e => e.Users)
                .UsingEntity<IdentityUserRole<string>>();
            
            b.HasOne(i => i.Boss)
                .WithMany(i => i.Children)
                .HasForeignKey(e => e.BossId);

            b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("user_name_index").IsUnique();
            b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("email_index");
            b.ToTable("asp_net_users");
        });
        
        modelBuilder.Entity<AuditLog>(b =>
        {
            b.HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        });
    }
}