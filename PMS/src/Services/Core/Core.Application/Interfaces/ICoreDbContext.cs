using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Interfaces;

public interface ICoreDbContext
{
    DbSet<Developer> Developers { get; }
    DbSet<ProjectTask> Tasks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
