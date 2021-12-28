using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MarshmallowPortal.OAuth2.Discord;
using MarshmallowPortal.OAuth2.Github;
using MarshmallowPortal.OAuth2.Google;
using ReactiveUI;

namespace MarshmallowPortal.Client.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private bool _logInActive;
    private readonly GoogleOAuth2Service _googleService;
    private readonly GithubOAuth2Service _githubService;
    private readonly DiscordOAuth2Service _discordService;
    private readonly string _state;
    private bool _addQuestionFlyoutEnabled;

    public bool LogInActive
    {
        get => _logInActive;
        set => this.RaiseAndSetIfChanged(ref _logInActive, value);
    }

    public bool AddQuestionFlyoutEnabled
    {
        get => _addQuestionFlyoutEnabled;
        set => this.RaiseAndSetIfChanged(ref _addQuestionFlyoutEnabled, value);
    }

    public void LoginWithGithubClicked()
    {
        var process = new ProcessStartInfo
        {
            FileName = _githubService.GetOAuth2Url(_state),
            UseShellExecute = true
        };
        Process.Start(process);
    }
    
    public void LoginWithDiscordClicked()
    {
        var process = new ProcessStartInfo
        {
            FileName = _discordService.GetOAuth2Url("http://localhost:6001/discord", _state),
            UseShellExecute = true
        };
        Process.Start(process);
    }
    
    public void LoginWithGoogleClicked()
    {
        var process = new ProcessStartInfo
        {
            FileName = _googleService.GetOAuth2Url("http://localhost:6001/google", _state),
            UseShellExecute = true
        };
        Process.Start(process);
    }

    public MainWindowViewModel()
    {
        LogInActive = true;
        var googleCredentials = new GoogleCredentials("1014028450593-3488rb7a1naenckr526ptl14t40ngl2f.apps.googleusercontent.com", null!);
        _googleService = new GoogleOAuth2Service(googleCredentials, new []{GoogleOAuth2Service.Email, GoogleOAuth2Service.Profile});
        var discordCredentials = new DiscordCredentials("914081312837607464", null!);
        _discordService = new DiscordOAuth2Service(discordCredentials, new []{DiscordOAuth2Service.Email, DiscordOAuth2Service.Identify});
        var githubCredentials = new GithubCredentials("bd50fdfd8c9cbef563da", null!);
        _githubService = new GithubOAuth2Service(githubCredentials, new []{GithubOAuth2Service.User});
        _state = _githubService.GetState();
        Task.Run(StartTheListener);
    }

    private async Task StartTheListener()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:6001/");
        listener.Start();
        var context = listener.GetContext();
        /*
        if (context.Request.QueryString["state"] != _state)
            throw new Exception("oh well");
        */
        var code = context.Request.QueryString["code"];
        var sr = new StreamWriter(context.Response.OutputStream);
        sr.Write("<h1>close</h1>");
        sr.Close();
        context.Response.Close();
        LogInActive = false;
    }
}