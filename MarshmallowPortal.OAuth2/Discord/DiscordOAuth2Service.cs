using System.Net;
using System.Security.Cryptography;
using MarshmallowPortal.OAuth2.Google;
using RestSharp;

namespace MarshmallowPortal.OAuth2.Discord;

public class DiscordOAuth2Service
{
    public DiscordCredentials Credentials { get; init; }
    public string[] Scopes { get; set; }

    public const string Email = "email";
    public const string Identify = "identify";

    public DiscordOAuth2Service(DiscordCredentials credentials, string[] scopes)
    {
        Credentials = credentials;
        Scopes = scopes;
    }

    public string GetOAuth2Url(string redirectUri, string state)
    {
        return
            @$"https://discord.com/api/oauth2/authorize?response_type=code&client_id={Credentials.ClientId}&scope=__SCOPES__&state={state}&redirect_uri={Uri.EscapeDataString(redirectUri)}&prompt=consent"
                .Replace("__SCOPES__", string.Join(" ", Scopes));
    }

    public string GetState()
    {
        return GenerateRandomDataBase64Url(32);
    }

    private static string GenerateRandomDataBase64Url(uint length)
    {
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        return Base64UrlEncodeNoPadding(bytes);
    }

    private static string Base64UrlEncodeNoPadding(byte[] buffer)
    {
        var base64 = Convert.ToBase64String(buffer);
        base64 = base64.Replace("+", "-");
        base64 = base64.Replace("/", "_");
        base64 = base64.Replace("=", "");
        return base64;
    }

    public async Task<DiscordUser> GetUser(string code, string redirectUri, bool renewToken = true,
        bool codeIsToken = false, string? refreshToken = null)
    {
        string? token, refreshToken1;
        if (!codeIsToken)
            (token, refreshToken1) = GetToken(code, redirectUri);
        else
            (token, refreshToken1) = (code, refreshToken);
        var user = new DiscordUser(token, refreshToken1!);
        await user.FetchProfile(); // making sure this token is valid
        if (!renewToken) return user;

        async void Callback(object? gUser)
        {
            if (gUser is GoogleUser googleUser) await RefreshToken(googleUser);
        }

        user.TokenRefreshTimer = new Timer(Callback, user, TimeSpan.FromSeconds(604799),
            TimeSpan.FromSeconds(604799));
        return user;
    }

    public Task RefreshToken(GoogleUser googleUser)
    {
        var client = new RestClient("https://discord.com/api/v8");
        var request = new RestRequest("oauth2/v4/token", Method.POST);
        request.AddJsonBody(new
        {
            client_id = Credentials.ClientId ?? throw new InvalidOperationException(),
            client_secret = Credentials.ClientSecret ?? throw new InvalidOperationException(),
            refresh_token = googleUser.RefreshToken,
            grant_type = "refresh_token"
        });
        var response = client.Execute<Dictionary<string, string>>(request).Data;
        var newToken = response["access_token"];
        googleUser.Token = newToken;
        return Task.CompletedTask;
    }

    public (string, string) GetToken(string code, string redirectUri)
    {
        var client = new RestClient("https://discord.com/api/v8");
        var request = new RestRequest("oauth2/token", Method.POST);
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("application/x-www-form-urlencoded",
            $"client_id={Credentials.ClientId}&client_secret={Credentials.ClientSecret}&grant_type=authorization_code&code={code}&redirect_uri={redirectUri}",
            ParameterType.RequestBody);
        try
        {
            var tokenEndpointDecoded = client.Execute<Dictionary<string, string>>(request).Data;
            return (tokenEndpointDecoded["access_token"] ?? throw new InvalidOperationException(),
                tokenEndpointDecoded["refresh_token"] ?? throw new InvalidOperationException());
        }
        catch (WebException ex)
        {
            if (ex.Status != WebExceptionStatus.ProtocolError) throw;
            if (ex.Response is HttpWebResponse response)
                throw new Exception($"https threw thi: {response.StatusDescription}");
            throw new Exception("some protocol error idk");
        }
    }
}