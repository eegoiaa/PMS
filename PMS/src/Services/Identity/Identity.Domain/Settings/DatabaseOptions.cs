namespace Identity.Domain.Settings;

public class DatabaseOptions
{
    public const string SectionName = "ConnectionStrings";
    public required string DefaultConnection { get; init; }
}
