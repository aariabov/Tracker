using Microsoft.EntityFrameworkCore;
using Tracker.Instructions.Db.Models;

namespace Tracker.Instructions.Db;

public class InstructionsDbContext : DbContext
{
    public DbSet<Instruction> Instructions => Set<Instruction>();
    public DbSet<InstructionClosure> InstructionsClosures => Set<InstructionClosure>();
    public DbSet<User> Users => Set<User>();

    public InstructionsDbContext(DbContextOptions<InstructionsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

            b.HasOne<Status>()
                .WithMany()
                .HasForeignKey(e => e.StatusId);
        });

        modelBuilder.Entity<InstructionClosure>(b =>
        {
            b.HasKey(i => new { i.ParentId, i.Id });

            b.HasOne<Instruction>()
                .WithMany()
                .HasForeignKey(e => e.ParentId);

            b.HasOne<Instruction>()
                .WithMany()
                .HasForeignKey(e => e.Id);
        });

        modelBuilder.Entity<User>(b =>
        {
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.BossId);

            b.HasOne(i => i.Boss)
                .WithMany(i => i.Children)
                .HasForeignKey(e => e.BossId);
        });
    }
}
