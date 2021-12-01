using RestSharp;

namespace MarshmallowPortal.OAuth2.Google;

public class GoogleUser
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
            if (!_profile.ContainsKey("name"))
                FetchProfile().GetAwaiter().GetResult();
            return _profile["name"];
        }
    }
    
    public string AvatarUrl
    {
        get
        {
            if (!_profile.ContainsKey("picture"))
                FetchProfile().GetAwaiter().GetResult();
            return _profile["picture"];
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

    internal GoogleUser(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }

    public Task FetchProfile()
    {
        var client = new RestClient("https://www.googleapis.com/");
        var request = new RestRequest("oauth2/v2/userinfo", Method.GET);
        request.AddParameter("access_token", Token, ParameterType.GetOrPost);
        _profile = client.Execute<Dictionary<string, string>>(request).Data;
        return Task.CompletedTask;
    }
}