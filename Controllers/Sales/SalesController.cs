using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace POS.Controllers.Sales;

[Authorize]
public class SalesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
