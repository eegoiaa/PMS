using Collector.Domain.Entities;

namespace Collector.Application.Interfaces;

public interface IActivityCleaner
{
    // Принимает сырой лог и пытается понять, к какой задаче (TaskId) он относится
    // Если это "шум" или фоновая активность, возвращает null
    Task<string?> ExtractTaskIdAsync(RawActivityLog rawLog, CancellationToken cancellationToken);

}
