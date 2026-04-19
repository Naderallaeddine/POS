using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace POS.Controllers.Reports;

[Authorize]
public class ReportsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
