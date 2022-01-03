using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarshmallowPortal.OAuth2.Discord;
using MarshmallowPortal.OAuth2.Github;
using MarshmallowPortal.OAuth2.Google;
using MarshmallowPortal.Shared;
using Microsoft.EntityFrameworkCore;

namespace MarshmallowPortal.Server.Data;

public class ApplicationContext : DbContext
{
    private readonly GoogleOAuth2Service _googleGoogleOAuth2Service;
    private readonly DiscordOAuth2Service _discordGoogleOAuth2Service;
    private readonly GithubOAuth2Service _githubGoogleOAuth2Service;
    private static readonly List<User> _users = new();

    public ApplicationContext(
        DbContextOptions<ApplicationContext> options,
        GoogleOAuth2Service googleGoogleOAuth2Service,
        DiscordOAuth2Service discordGoogleOAuth2Service,
        GithubOAuth2Service githubGoogleOAuth2Service) : base(options)
    {
        _googleGoogleOAuth2Service = googleGoogleOAuth2Service;
        _discordGoogleOAuth2Service = discordGoogleOAuth2Service;
        _githubGoogleOAuth2Service = githubGoogleOAuth2Service;
    }

    public async Task<User> LoginUser(string code, TokenType type)
    {
        switch (type)
        {
            case TokenType.Google:
                var user0 = await _googleGoogleOAuth2Service.GetUser(code, "http://localhost:6001/google", false);
                User u0 = user0;
                u0.TokenLifetime = 1;
                _users.Add(u0);
                return u0;
            case TokenType.Github:
                var user1 = await _githubGoogleOAuth2Service.GetUser(code);
                User u1 = user1;
                u1.TokenLifetime = -1;
                _users.Add(u1);
                return u1;
            case TokenType.Discord:
                var user2 = await _discordGoogleOAuth2Service.GetUser(code, "http://localhost:6001/discord", false);
                User u2 = user2;
                u2.TokenLifetime = 1;
                _users.Add(u2);
                return u2;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public async Task<string> RefreshToken(string reqToken, string reqRefreshToken, TokenType tokenType)
    {
        if (_users.All(x => x.Token != reqToken))
            throw new Exception("who are you");
        return tokenType switch
        {
            TokenType.Google => _googleGoogleOAuth2Service.RefreshToken(reqRefreshToken),
            TokenType.Github => throw new Exception("no"),
            TokenType.Discord => _discordGoogleOAuth2Service.RefreshToken(reqRefreshToken),
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
        };
    }
}