using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.OAuth2;
using MarshmallowPortal.Server.Options;
using MarshmallowPortal.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MarshmallowPortal.Server.Data;

public class ApplicationContext : DbContext
{
    private static readonly Dictionary<string, string> _tokensToIdsGoogle = new();
    private static readonly Dictionary<string, UserEntity> _idsToUsersGoogle = new();
    private static OAuth2Service _oAuth2ServiceGoogle;
    private static IOptions<GoogleOptions> _googleOptions;

    public ApplicationContext(
        DbContextOptions<ApplicationContext> options,
        OAuth2Service oAuth2ServiceGoogle,
        IOptions<GoogleOptions> googleOptions
        ) : base(options)
    {
        _oAuth2ServiceGoogle = oAuth2ServiceGoogle;
        _googleOptions = googleOptions;
    }

    public async Task<UserEntity> GetUser(string token, TokenType type)
    {
        switch (type)
        {
            case TokenType.Google:
                _tokensToIdsGoogle.TryGetValue(token, out var user);
                if (user is not null) return _idsToUsersGoogle[user];
                var newGoogleUser = await _oAuth2ServiceGoogle.GetUser(token, _googleOptions.Value.RedirectUri, false, true);
                _ = newGoogleUser.Id;
                var newUser = new UserEntity
                {
                    Token = newGoogleUser.Token,
                    Id = newGoogleUser.Id,
                    Username = newGoogleUser.Username,
                    AvatarUrl = newGoogleUser.AvatarUrl
                };
                _tokensToIdsGoogle.Add(token, newUser.Id);
                _idsToUsersGoogle.Add(newUser.Id, newUser);
                return newUser;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public async Task<UserEntity> UpdateUserToken(string oldToken, string newToken, TokenType type)
    {
        switch (type)
        {
            case TokenType.Google:
                if (!_tokensToIdsGoogle.ContainsKey(oldToken)) throw new Exception("no");
                // just making sure token is valid
                var user = await _oAuth2ServiceGoogle.GetUser(newToken, _googleOptions.Value.RedirectUri, false, true);
                var id = _tokensToIdsGoogle[oldToken];
                if (user.Id != id) throw new Exception("what");
                _tokensToIdsGoogle.Remove(oldToken);
                _tokensToIdsGoogle.Add(newToken, id);
                return _idsToUsersGoogle[id];
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}