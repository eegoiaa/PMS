using Core.Application.Interfaces;
using Core.Domain.Entities;
using PMS.Shared.Common.Constants;
using PMS.Shared.Common.Events;

namespace Core.Application.Consumers;

public static class UserRegisteredConsumer
{
    public static async Task Handle(
        UserRegisteredEvent message,
        ICoreDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (message.Role != RolesConstants.Developer && message.Role != RolesConstants.ProjectManager) return;

        var developer = new Developer
        {
            Id = message.UserId,
            FullName = message.FullName,
            AverageVolatility = 0
        };

        dbContext.Developers.Add(developer);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
