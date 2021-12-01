namespace MarshmallowPortal.OAuth2.Github;

public class GithubCredentials
{
    public GithubCredentials(string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }

    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}