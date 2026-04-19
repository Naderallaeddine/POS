using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace POS.Controllers.Purchases;

[Authorize]
public class PurchasesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
