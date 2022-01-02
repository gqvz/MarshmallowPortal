using System.IO;
using MarshmallowPortal.Shared;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using User = MarshmallowPortal.Shared.User;

namespace MarshmallowPortal.Client;

public record Config
{
    public ServerConfig Server { get; init; }
}

public record ServerConfig
{
    public string Address { get; init; }
    public int Port { get; init; }
}

public class MarshmallowClient
{
    private Config _config;
    private RestClient _client;


    public MarshmallowClient()
    {
        if (!File.Exists("Config.json"))
        {
            new StreamWriter(File.Create("Config.json"))
                .Write("{\n  \"Server\": {\n    \"Address\": \"127.0.0.1\",\n    \"Port\": 5001\n  }");
        }

        _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Config.json"));

        _client = new RestClient($"{_config.Server.Address}:{_config.Server.Port}");
    }
    
    public void AddAuth(string token)
    {
        _client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token);
    }

    public User GetUser(string code, TokenType tokenType)
    {
        var request = new RestRequest($"api/login/{tokenType}", Method.GET);
        request.AddQueryParameter("code", code);
        var res = _client.Get<User>(request);
        return res.Data;
    }
}