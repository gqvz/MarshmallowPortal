using MarshmallowPortal.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace MarshmallowPortal.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly ApplicationContext _context;

    public LoginController(ApplicationContext context)
    {
        _context = context;
    }
    
    [HttpGet("index")]
    public IActionResult Index()
    {
        return Unauthorized();
    }
}