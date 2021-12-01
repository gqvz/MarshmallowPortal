using MarshmallowPortal.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace MarshmallowPortal.Server.Controllers;

[ApiController]
[Route("/api/token")]
public class TokenController : ControllerBase
{
    private readonly ApplicationContext _context;

    public TokenController(ApplicationContext context)
    {
        _context = context;
    } 
}