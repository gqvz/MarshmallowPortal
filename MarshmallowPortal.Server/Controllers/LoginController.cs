using System;
using System.Threading.Tasks;
using MarshmallowPortal.Server.Data;
using MarshmallowPortal.Shared;
using Microsoft.AspNetCore.Mvc;

namespace MarshmallowPortal.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ApplicationContext _context;

    public LoginController(ApplicationContext context)
    {
        _context = context;
    }
    
    [HttpPost("google")] // i think i'll be adding a few more idk
    public async Task<LoginResponse> Google([FromBody] LoginRequest request)
    {
        try
        {
            await _context.GetUser(request.Token, request.TokenType);
            return new LoginResponse
            {
                Success = true
            };
        }
        catch (Exception e)
        {
            return new LoginResponse
            {
                Success = false,
                Error = e.Message
            };
        }
    }
    
    [HttpPost("google/refresh")]
    public async Task<LoginResponse> GoogleRefresh([FromBody] UpdateTokenRequest request)
    {
        try
        {
            await _context.UpdateUserToken(request.OldToken, request.Token, request.TokenType);
            return new LoginResponse
            {
                Success = true
            };
        }
        catch (Exception e)
        {
            return new LoginResponse
            {
                Success = false,
                Error = e.Message
            };
        }
    }
}