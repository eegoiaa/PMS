using Core.Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence;

public class CoreDbContext : DbContext, ICoreDbContext
{
    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }
    
    public DbSet<Developer> Developers { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Title).IsRequired().HasMaxLength(500);

            entity.HasOne(t => t.Developer)
                .WithMany(d => d.Tasks)
                .HasForeignKey(t => t.DeveloperId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
