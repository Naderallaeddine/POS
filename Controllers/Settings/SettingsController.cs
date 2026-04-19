using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace POS.Controllers.Settings;

[Authorize(Policy = "RequireAdministrator")]
public class SettingsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
