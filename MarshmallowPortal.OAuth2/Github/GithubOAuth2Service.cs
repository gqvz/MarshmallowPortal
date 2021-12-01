using System.Net;
using System.Security.Cryptography;
using RestSharp;

namespace MarshmallowPortal.OAuth2.Github;

public sealed class GithubOAuth2Service
{
    public GithubCredentials Credentials { get; init; }
    public string[] Scopes { get; set; }
    
    public const string UserReadonly = "read:user";
    public const string EmailReadonly = "user:email";
    public const string User = "user";

    public GithubOAuth2Service(GithubCredentials credentials, string[] scopes)
    {
        Credentials = credentials;
        Scopes = scopes;
    }

    public async Task<GithubUser> GetUser(string code, bool codeIsToken = false, string? refreshToken = null)
    {
        var token = !codeIsToken ? GetToken(code) : code;
        var user = new GithubUser(token);
        await user.FetchProfile(); // making sure this token is valid
        return user;
    }
    
    public string GetToken(string code)
    {
        var client = new RestClient("https://github.com/login/oauth/access_token");
        var request = new RestRequest(Method.POST);
        request.AddJsonBody(new
        {
            client_id = Credentials.ClientId ?? throw new InvalidOperationException(),
            client_secret = Credentials.ClientSecret,
            code,
        });
        try
        {
            var tokenEndpointDecoded = client.Execute<Dictionary<string, string>>(request).Data;
            return tokenEndpointDecoded["access_token"] ?? throw new InvalidOperationException();
        }
        catch (WebException ex)
        {
            if (ex.Status != WebExceptionStatus.ProtocolError) throw;
            if (ex.Response is HttpWebResponse response)
                throw new Exception($"https threw thi: {response.StatusDescription}");
            throw new Exception("some protocol error idk");
        }
    }

    public string GetOAuth2Url(string state)
    {
        return string.Format($@"https://github.com/login/oauth/authorize?
state={state}&
scope=__SCOPES__&
prompt=consent&
client_id={Credentials.ClientId}".Replace(" ", "")
            .Replace("__SCOPES__", string.Join(",", Scopes.Select(x => x.Replace(":", "%2F")) ?? throw new InvalidOperationException())));
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