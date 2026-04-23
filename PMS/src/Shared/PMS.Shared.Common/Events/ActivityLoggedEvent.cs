namespace PMS.Shared.Common.Events;

public record ActivityLoggedEvent(
    string TaskKey, 
    string AuthorEmail,
    double SpentHours
    );

