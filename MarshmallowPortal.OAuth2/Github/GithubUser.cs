using RestSharp;

namespace MarshmallowPortal.OAuth2.Github;

public class GithubUser
{
    public string Token { get; set; }
    
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
            if (!_profile.ContainsKey("avatar_url"))
                FetchProfile().GetAwaiter().GetResult();
            return _profile["avatar_url"];
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

    internal GithubUser(string token)
    {
        Token = token;
    }

    public Task FetchProfile()
    {
        var client = new RestClient("https://api.github.com/user");
        var request = new RestRequest(Method.GET);
        var request2 = new RestRequest("emails", Method.GET);
        request.AddHeader("Authorization", $"token {Token}");
        request2.AddHeader("Authorization", $"token {Token}");
        _profile = client.Execute<Dictionary<string, string>>(request).Data;
        var dict2 = client.Execute<List<Dictionary<string, object>>>(request2).Data;
        _profile["email"] = dict2.FirstOrDefault(x => (bool) x["primary"])?["email"] as string ?? throw new InvalidCastException();
        return Task.CompletedTask;
    }
}