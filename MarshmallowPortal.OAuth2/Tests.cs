using System.Diagnostics;
using System.Net;
using MarshmallowPortal.OAuth2.Google;
using MarshmallowPortal.OAuth2.Discord;
using MarshmallowPortal.OAuth2.Github;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MarshmallowPortal.OAuth2;

#pragma warning disable CS8618
// ReSharper disable UnusedAutoPropertyAccessor.Global
public record Config
{
    public GithubConfig Github { get; init; }
    public DiscordConfig Discord { get; init; }
    public GoogleConfig Google { get; init; }
    
    public record GithubConfig
    {
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
    }

    public record DiscordConfig
    {
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
    }

    public record GoogleConfig
    {
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
    }
}
// ReSharper restore UnusedAutoPropertyAccessor.Global
#pragma warning restore CS8618

public class Tests
{
    private GoogleOAuth2Service _googleGoogleOAuth2Service = null!;
    private DiscordOAuth2Service _discordDiscordOAuth2Service = null!;
    private GithubOAuth2Service _githubGithubOAuth2Service = null!;
    
    [SetUp]
    public void Setup()
    {
        var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Config.json"));
        
        var googleCredentials = new GoogleCredentials(config.Google.ClientId, config.Google.ClientSecret);
        _googleGoogleOAuth2Service = new GoogleOAuth2Service(googleCredentials, new []{GoogleOAuth2Service.Email, GoogleOAuth2Service.Profile});
        var discordCredentials = new DiscordCredentials(config.Discord.ClientId, config.Discord.ClientSecret);
        _discordDiscordOAuth2Service = new DiscordOAuth2Service(discordCredentials, new []{DiscordOAuth2Service.Identify, DiscordOAuth2Service.Email});
        var githubCredentials = new GithubCredentials(config.Github.ClientId, config.Github.ClientSecret);
        _githubGithubOAuth2Service = new GithubOAuth2Service(githubCredentials, new []{GithubOAuth2Service.User});
    }

    [Test]
    public async Task Google()
    {
        var state = _googleGoogleOAuth2Service.GetState();
        var url = _googleGoogleOAuth2Service.GetOAuth2Url("http://localhost:6001/google", state);
        Process.Start(new ProcessStartInfo(url) {UseShellExecute = true});
        using var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:6001/google/");
        listener.Start();
        var context = await listener.GetContextAsync();
        var code = context.Request.QueryString["code"];
        var state2 = context.Request.QueryString["state"];
        if (state != state2)
            throw new Exception("State mismatch");
        Console.WriteLine(code);
        var sw = new StreamWriter(context.Response.OutputStream);
        await sw.WriteAsync("<h1>close me</h1>");
        sw.Close();
        context.Response.Close();
        listener.Stop();
        var user = await _googleGoogleOAuth2Service.GetUser(code!, "http://localhost:6001/google");
        Console.WriteLine(user.Token);
        Console.WriteLine(user.Email);
        Console.WriteLine(user.Username);
        Console.WriteLine(user.Id);
        Console.WriteLine(user.AvatarUrl);
    }
    
    [Test]
    public async Task Discord()
    {
        var state = _discordDiscordOAuth2Service.GetState();
        var url = _discordDiscordOAuth2Service.GetOAuth2Url("http://localhost:6001/discord", state);
        Process.Start(new ProcessStartInfo(url) {UseShellExecute = true});
        using var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:6001/discord/");
        listener.Start();
        var context = await listener.GetContextAsync();
        var code = context.Request.QueryString["code"];
        var state2 = context.Request.QueryString["state"];
        if (state != state2)
            throw new Exception("State mismatch");
        Console.WriteLine(code);
        var sw = new StreamWriter(context.Response.OutputStream);
        await sw.WriteAsync("<h1>close me</h1>");
        sw.Close();
        context.Response.Close();
        listener.Stop();
        var user = await _discordDiscordOAuth2Service.GetUser(code!, "http://localhost:6001/discord");
        Console.WriteLine(user.Token);
        Console.WriteLine(user.Email);
        Console.WriteLine(user.Username);
        Console.WriteLine(user.Id);
        Console.WriteLine(user.AvatarUrl);
    }
    
    [Test]
    public async Task Github()
    {
        var state = _githubGithubOAuth2Service.GetState();
        var url = _githubGithubOAuth2Service.GetOAuth2Url(state);
        Process.Start(new ProcessStartInfo(url) {UseShellExecute = true});
        using var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:6001/github/");
        listener.Start();
        var context = await listener.GetContextAsync();
        var code = context.Request.QueryString["code"];
        var state2 = context.Request.QueryString["state"];
        if (state != state2)
            throw new Exception("State mismatch");
        Console.WriteLine(code);
        var sw = new StreamWriter(context.Response.OutputStream);
        await sw.WriteAsync("<h1>close me</h1>");
        sw.Close();
        context.Response.Close();
        listener.Stop();
        var user = await _githubGithubOAuth2Service.GetUser(code!);
        Console.WriteLine(user.Token);
        Console.WriteLine(user.Email);
        Console.WriteLine(user.Username);
        Console.WriteLine(user.Id);
        Console.WriteLine(user.AvatarUrl);
    }
}