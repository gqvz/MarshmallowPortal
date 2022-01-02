using MarshmallowPortal.OAuth2.Discord;
using MarshmallowPortal.OAuth2.Github;
using MarshmallowPortal.OAuth2.Google;

namespace MarshmallowPortal.Shared;

public class User
{
    public string Token { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int TokenLifetime { get; set; } = 0;

    public static implicit operator User(GoogleUser gu)
    {
        return new User
        {
            AvatarUrl = gu.AvatarUrl,
            Email = gu.Email,
            Id = gu.Id,
            Token = gu.Token,
            Username = gu.Username,
            RefreshToken = gu.RefreshToken
        };
    }
    
    public static implicit operator User(DiscordUser gu)
    {
        return new User
        {
            AvatarUrl = gu.AvatarUrl,
            Email = gu.Email,
            Id = gu.Id,
            Token = gu.Token,
            Username = gu.Username,
            RefreshToken = gu.RefreshToken
        };
    }

    public static implicit operator User(GithubUser gu)
    {
        return new User
        {
            AvatarUrl = gu.AvatarUrl,
            Email = gu.Email,
            Id = gu.Id,
            Token = gu.Token,
            Username = gu.Username,
            RefreshToken = null!
        };
    }
}