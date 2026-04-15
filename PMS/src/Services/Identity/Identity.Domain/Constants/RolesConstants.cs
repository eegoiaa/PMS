namespace Identity.Domain.Constants;

public static class RolesConstants
{
    public const string Admin = "Admin";
    public const string Developer = "Developer";
    public const string ProjectManager = "ProjectManager";

    public static readonly Guid AdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid DeveloperId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid ProjectManagerId = Guid.Parse("33333333-3333-3333-3333-333333333333");
}
