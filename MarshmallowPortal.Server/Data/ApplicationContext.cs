using Google.OAuth2;
using MarshmallowPortal.Server.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MarshmallowPortal.Server.Data;

public class ApplicationContext : DbContext
{
    private readonly Dictionary<string, string> _tokensToIds = new();
    private readonly Dictionary<string, UserEntity> _idsToUsers = new();
    private readonly OAuth2Service _oAuth2Service;
    private readonly IOptions<GoogleOptions> _googleOptions;

    public ApplicationContext(
        DbContextOptions<ApplicationContext> options,
        OAuth2Service oAuth2Service,
        IOptions<GoogleOptions> googleOptions
        ) : base(options)
    {
        _oAuth2Service = oAuth2Service;
        _googleOptions = googleOptions;
    }

    public async Task<UserEntity> GetUser(string token)
    {
        _tokensToIds.TryGetValue(token, out var user);
        if (user is not null) return _idsToUsers[user];
        var newGoogleUser = await _oAuth2Service.GetUser(token, _googleOptions.Value.RedirectUri, false, true);
        var newUser = new UserEntity
        {
            Token = newGoogleUser.Token,
            Id = newGoogleUser.Id,
            Username = newGoogleUser.Username,
            AvatarUrl = newGoogleUser.AvatarUrl
        };
        _tokensToIds.Add(token, newUser.Id);
        _idsToUsers.Add(newUser.Id, newUser);
        return newUser;
    }

    public async Task<UserEntity> UpdateUserToken(string oldToken, string newToken)
    {
        if (!_tokensToIds.ContainsKey(oldToken)) throw new Exception("no");
        // just making sure token is valid
        var user = await _oAuth2Service.GetUser(newToken, _googleOptions.Value.RedirectUri, false, true);
        var id = _tokensToIds[oldToken];
        if (user.Id != id) throw new Exception("what");
        _tokensToIds.Remove(oldToken);
        _tokensToIds.Add(newToken, id);
        return _idsToUsers[id];
    }
}