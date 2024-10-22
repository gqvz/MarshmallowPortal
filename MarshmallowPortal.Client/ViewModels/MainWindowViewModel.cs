﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using MarshmallowPortal.OAuth2.Discord;
using MarshmallowPortal.OAuth2.Github;
using MarshmallowPortal.OAuth2.Google;
using MarshmallowPortal.Shared;
using ReactiveUI;
using Serilog;

namespace MarshmallowPortal.Client.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly MarshmallowClient _client;
    private bool _logInActive;
    private readonly GoogleOAuth2Service _googleService;
    private readonly GithubOAuth2Service _githubService;
    private readonly DiscordOAuth2Service _discordService;
    private readonly string _state;
    private User _user = null!;
    private string _loginScreenText = "Please login using your browser";

    public bool LogInActive
    {
        get => _logInActive;
        set => this.RaiseAndSetIfChanged(ref _logInActive, value);
    }
    
    public string LoginScreenText
    {
        get => _loginScreenText;
        set => this.RaiseAndSetIfChanged(ref _loginScreenText, value);
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
        _client = new MarshmallowClient();
        LogInActive = false;
        var googleCredentials = new GoogleCredentials("171949902476-fskne98irdh28pu9cqen310hblhg1f2v.apps.googleusercontent.com", null!);
        _googleService = new GoogleOAuth2Service(googleCredentials, new []{GoogleOAuth2Service.Email, GoogleOAuth2Service.Profile});
        var discordCredentials = new DiscordCredentials("914081312837607464", null!);
        _discordService = new DiscordOAuth2Service(discordCredentials, new []{DiscordOAuth2Service.Email, DiscordOAuth2Service.Identify});
        var githubCredentials = new GithubCredentials("bd50fdfd8c9cbef563da", null!);
        _githubService = new GithubOAuth2Service(githubCredentials, new []{GithubOAuth2Service.User});
        _state = _githubService.GetState();
        Task.Run(StartTheListener);
    }

    private Task StartTheListener()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:6001/");

        listener.Start();
        var context = listener.GetContext();
        if (context.Request.QueryString["state"] != _state)
        {
            LoginScreenText = "shit went wild";
            Log.Error("state no no");
            var s = new StreamWriter(context.Response.OutputStream);
            s.Write("<h1>smh epic error</h1>");
            s.Close();
            context.Response.Close();
            listener.Stop();
            Task.Run(StartTheListener);
            return Task.CompletedTask;
        }
        var code = context.Request.QueryString["code"];
        TokenType tokenType;
        try
        {
            tokenType = context.Request.Url?.AbsolutePath[1..] switch
            {
                "discord" => TokenType.Discord,
                "google" => TokenType.Google,
                "github" => TokenType.Github,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        catch
        {
            LoginScreenText = "shit went wild";
            Log.Error("what ya doin bruh");
            var s = new StreamWriter(context.Response.OutputStream);
            s.Write("<h1>smh epic error</h1>");
            s.Close();
            context.Response.Close();
            listener.Stop();
            Task.Run(StartTheListener);
            return Task.CompletedTask;
        }
        var sr = new StreamWriter(context.Response.OutputStream);
        sr.Write("<h1>close</h1>");
        sr.Close();
        context.Response.Close();
        listener.Stop();
        try
        {
            _user = _client.GetUser(code, tokenType);
        }
        catch (Exception e)
        {
            LoginScreenText = "shit went wild";
            Log.Error(e, "Error lol");
            Task.Run(StartTheListener);
            return Task.CompletedTask;
        }
        LogInActive = false;
        return Task.CompletedTask;
    }
}