namespace MarshmallowPortal.OAuth2.Discord;

public sealed class DiscordCredentials
{
    public readonly string ClientId;
    public readonly string ClientSecret;

    public DiscordCredentials(string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }
}