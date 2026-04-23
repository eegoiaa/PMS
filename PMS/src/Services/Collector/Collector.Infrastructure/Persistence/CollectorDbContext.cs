using Collector.Application.Interfaces;
using Collector.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Collector.Infrastructure.Persistence;

public class CollectorDbContext : DbContext, ICollectorDbContext
{
    public CollectorDbContext(DbContextOptions<CollectorDbContext> options) : base(options) { }

    public DbSet<RawActivityLog> RawActivityLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RawActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Source).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AuthorEmail).IsRequired().HasMaxLength(256);
        });
    }
}
