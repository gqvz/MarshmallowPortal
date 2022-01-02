using System.Threading.Tasks;
using MarshmallowPortal.Server.Data;
using MarshmallowPortal.Shared;
using Microsoft.AspNetCore.Mvc;

namespace MarshmallowPortal.Server.Controllers;

[ApiController]
[Route("/api/login")]
public class TokenController : ControllerBase
{
    private readonly ApplicationContext _context;

    public TokenController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet("discord")]
    public async Task<JsonResult> GET_Discord([FromQuery] LoginRequest req)
    {
        return new JsonResult(await _context.LoginUser(req.Code, TokenType.Discord));
    }
    
    [HttpGet("github")]
    public async Task<JsonResult> GET_Github([FromQuery] LoginRequest req)
    {
        return new JsonResult(await _context.LoginUser(req.Code, TokenType.Github));
    }

    [HttpGet("google")]
    public async Task<JsonResult> GET_Google([FromQuery] LoginRequest req)
    {
        return new JsonResult(await _context.LoginUser(req.Code, TokenType.Google));
    }
}