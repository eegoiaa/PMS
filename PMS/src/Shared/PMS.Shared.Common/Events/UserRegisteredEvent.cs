namespace PMS.Shared.Common.Events;

public record UserRegisteredEvent(
    Guid UserId,
    string FullName,
    string Role
    );
