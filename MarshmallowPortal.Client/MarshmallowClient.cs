using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarshmallowPortal.Shared;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using User = MarshmallowPortal.Shared.User;

namespace MarshmallowPortal.Client;

public record Config
{
    public ServerConfig[] Servers { get; init; }
}

public record ServerConfig
{
    public string Address { get; init; }
    public int Port { get; init; }
    public bool Active { get; init; }
}

public class MarshmallowClient
{
    private readonly Config _config;
    private readonly RestClient _client;
    private User _currentUser;

    public MarshmallowClient()
    {
        if (!File.Exists("ClientConfig.json"))
        {
            new StreamWriter(File.Create("Config.json"))
                .Write("{\n  \"Server\": {\n    \"Address\": \"127.0.0.1\",\n    \"Port\": 5001\n  }");
        }

        _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("ClientConfig.json"));

        var server = _config.Servers.FirstOrDefault(x => x.Active) ?? _config.Servers.First();
        
        _client = new RestClient($"{server.Address}{(server.Port == 0 ? "" : $":{server.Port}")}");
    }
    
    public User GetUser(string code, TokenType tokenType)
    {
        var request = new RestRequest($"api/login/{tokenType}", Method.GET);
        request.AddQueryParameter("code", code);
        var result = _client.Get<User>(request);

        _client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(result.Data.Token);
        
        if (tokenType == TokenType.Github) return result.Data;

        _currentUser = result.Data;
        
        void CallbackForRefresh(User u)
        {
            var req = new RestRequest($"api/login/{tokenType}", Method.POST);
            req.AddJsonBody(new TokenRefreshRequest(u.Token, u.RefreshToken));
            var res = _client.Post<string>(req);
            u.Token = res.Data;
            Task.Delay(u.TokenLifetime * 1000).ContinueWith(_ => CallbackForRefresh(u));
        }
        Task.Delay(result.Data.TokenLifetime * 1000).ContinueWith(_ => CallbackForRefresh(result.Data));
        return result.Data;
    }
}