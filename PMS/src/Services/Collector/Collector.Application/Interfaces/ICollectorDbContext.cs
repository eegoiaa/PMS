using Collector.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Collector.Application.Interfaces;

public interface ICollectorDbContext
{
    DbSet<RawActivityLog> RawActivityLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
