using Microsoft.AspNetCore.Mvc;

namespace MarshmallowPortal.Server.Controllers;

[Controller]
public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }
}