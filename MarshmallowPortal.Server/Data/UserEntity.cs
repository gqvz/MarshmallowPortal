namespace MarshmallowPortal.Server.Data;

public class UserEntity
{
    public string Token { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
}