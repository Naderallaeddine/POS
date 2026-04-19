using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.DTOs.Sales;
using POS.Interfaces.Services;
using POS.ViewModels.Sales;

namespace POS.Controllers.Sales;

[Authorize(Policy = "RequireSalesAccess")]
public class SalesController : Controller
{
    private readonly ISalesService _salesService;

    public SalesController(ISalesService salesService)
    {
        _salesService = salesService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new PosViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < 2)
        {
            return Json(Array.Empty<ProductSearchResultVm>());
        }

        var results = await _salesService.SearchProductsAsync(term, cancellationToken: cancellationToken);
        return Json(results);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout([FromBody] SaleCreateDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var saleId = await _salesService.CreateSaleAsync(dto, cancellationToken);
            return Json(new { ok = true, saleId });
        }
        catch (InvalidOperationException ex)
        {
            Response.StatusCode = 400;
            return Json(new { ok = false, message = ex.Message });
        }
    }
}
