namespace WreFigDemo.Identity;

public class AppUserResourceType
{
    public string UserId           { get; set; } = string.Empty;
    public AppUser User            { get; set; } = null!;
    public string ResourceTypeName { get; set; } = string.Empty;
}
