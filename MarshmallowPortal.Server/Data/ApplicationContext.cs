using MarshmallowPortal.OAuth2.Discord;
using MarshmallowPortal.OAuth2.Github;
using MarshmallowPortal.OAuth2.Google;
using MarshmallowPortal.Server.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MarshmallowPortal.Server.Data;

public class ApplicationContext : DbContext
{
    private static GoogleOAuth2Service _googleGoogleOAuth2Service = null!;
    private static DiscordOAuth2Service _discordGoogleOAuth2Service = null!;
    private static GithubOAuth2Service _githubGoogleOAuth2Service = null!;
    private static IOptions<GoogleOptions> _googleOptions = null!;
    private static IOptions<DiscordOptions> _discordOptions = null!;
    private static IOptions<GithubOptions> _githubOptions = null!;

    public ApplicationContext(
        DbContextOptions<ApplicationContext> options,
        GoogleOAuth2Service googleGoogleOAuth2Service,
        DiscordOAuth2Service discordGoogleOAuth2Service,
        GithubOAuth2Service githubGoogleOAuth2Service,
        IOptions<GoogleOptions> googleOptions,
        IOptions<DiscordOptions> discordOptions,
        IOptions<GithubOptions> githubOptions) : base(options)
    {
        _googleGoogleOAuth2Service = googleGoogleOAuth2Service;
        _discordGoogleOAuth2Service = discordGoogleOAuth2Service;
        _githubGoogleOAuth2Service = githubGoogleOAuth2Service;
        _googleOptions = googleOptions;
        _discordOptions = discordOptions;
        _githubOptions = githubOptions;
    }
}