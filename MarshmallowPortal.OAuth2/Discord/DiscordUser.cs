using RestSharp;

namespace MarshmallowPortal.OAuth2.Discord;

public class DiscordUser
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public Timer? TokenRefreshTimer { get; set; }
    
    public string Email
    {
        get
        {
            if (!_profile.ContainsKey("email"))
                FetchProfile().GetAwaiter().GetResult();
            return _profile["email"];
        }
    }
    
    public string Username
    {
        get
        {
            if (!_profile.ContainsKey("username"))
                FetchProfile().GetAwaiter().GetResult();
            return _profile["username"];
        }
    }
    
    public string AvatarUrl
    {
        get
        {
            if (!_profile.ContainsKey("avatar"))
                FetchProfile().GetAwaiter().GetResult();
            return $"https://cdn.discordapp.com/avatars/{Id}/{_profile["avatar"]}.jpg";
        }
    }
    
    public string Id
    {
        get
        {
            if (!_profile.ContainsKey("id"))
                FetchProfile().GetAwaiter().GetResult();
            return _profile["id"];
        }
    }

    private Dictionary<string, string> _profile = new();

    internal DiscordUser(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }

    public Task FetchProfile()
    {
        var client = new RestClient("https://discord.com/api/users/@me");
        var request = new RestRequest(Method.GET);
        request.AddHeader("Authorization", $"Bearer {Token}");
        _profile = client.Execute<Dictionary<string, string>>(request).Data;
        return Task.CompletedTask;
    }
}