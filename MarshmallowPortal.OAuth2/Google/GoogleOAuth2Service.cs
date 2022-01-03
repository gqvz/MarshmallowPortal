using System.Net;
using System.Security.Cryptography;
using RestSharp;

namespace MarshmallowPortal.OAuth2.Google;

public sealed class GoogleOAuth2Service
{
    public GoogleCredentials Credentials { get; init; }
    public string[] Scopes { get; set; }
    
    public const string Email = "https%3A//www.googleapis.com/auth/userinfo.email";
    public const string Profile = "https%3A//www.googleapis.com/auth/userinfo.profile";
    public const string OpenId = "openid";
    public const string TokenRequestUri = "https://www.googleapis.com";

    public GoogleOAuth2Service(GoogleCredentials credentials, string[] scopes)
    {
        Credentials = credentials;
        Scopes = scopes;
    }

    public async Task<GoogleUser> GetUser(string code, string redirectUri, bool renewToken = true, bool codeIsToken = false, string? refreshToken = null)
    {
        string? token, refreshToken1;
        if (!codeIsToken)
            (token, refreshToken1) = GetToken(code, redirectUri);
        else
            (token, refreshToken1) = (code, refreshToken);
        var user = new GoogleUser(token, refreshToken1!);
        await user.FetchProfile(); // making sure this token is valid
        if (!renewToken) return user;

        void Callback(object? gUser)
        {
            if (gUser is GoogleUser googleUser)
            {
                googleUser.Token = RefreshToken(googleUser.RefreshToken);
            }
        }

        user.TokenRefreshTimer = new Timer(Callback, user, TimeSpan.FromSeconds(3599),
            TimeSpan.FromSeconds(3599));
        return user;
    }

    public string RefreshToken(string refreshToken)
    {
        var client = new RestClient(TokenRequestUri);
        var request = new RestRequest("oauth2/v4/token", Method.POST);
        request.AddJsonBody(new
        {
            client_id = Credentials.ClientId ?? throw new InvalidOperationException(),
            client_secret = Credentials.ClientSecret ?? throw new InvalidOperationException(),
            refresh_token = refreshToken,
            grant_type = "refresh_token"
        });
        var response = client.Execute<Dictionary<string, string>>(request).Data;
        var newToken = response["access_token"];
        return newToken;
    }

    public (string, string) GetToken(string code, string redirectUri)
    {
        var client = new RestClient(TokenRequestUri);
        var request = new RestRequest("oauth2/v4/token", Method.POST);
        request.AddJsonBody(new
        {
            client_id = Credentials.ClientId ?? throw new InvalidOperationException(),
            client_secret = Credentials.ClientSecret,
            code,
            redirect_uri = redirectUri,
            grant_type = "authorization_code",
            scope = ""
        });
        try
        {
            var tokenEndpointDecoded = client.Execute<Dictionary<string, string>>(request).Data;
            return (tokenEndpointDecoded["access_token"] ?? throw new InvalidOperationException(), tokenEndpointDecoded["refresh_token"] ?? throw new InvalidOperationException());
        }
        catch (WebException ex)
        {
            if (ex.Status != WebExceptionStatus.ProtocolError) throw;
            if (ex.Response is HttpWebResponse response)
                throw new Exception($"https threw thi: {response.StatusDescription}");
            throw new Exception("some protocol error idk");
        }
    }

    public string GetOAuth2Url(string redirect, string state)
    {
        return string.Format($@"https://accounts.google.com/o/oauth2/v2/auth?
response_type=code&
access_type=offline&
state={state}&
redirect_uri={redirect}&
scope=__SCOPES__&
prompt=consent&
client_id={Credentials.ClientId}".Replace(" ", "")
            .Replace("__SCOPES__", string.Join(" ", Scopes.Select(x => x.Replace(":", "%2F")) ?? throw new InvalidOperationException())));
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
}