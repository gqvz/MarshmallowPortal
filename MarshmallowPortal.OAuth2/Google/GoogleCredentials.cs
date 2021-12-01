namespace MarshmallowPortal.OAuth2.Google;


public sealed class GoogleCredentials
{
    public readonly string ClientId;
    public readonly string ClientSecret;

    public GoogleCredentials(string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }
}