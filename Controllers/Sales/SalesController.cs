using System.Text.Encodings.Web;
using System.Text.Json;
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
    private readonly IStoreSettingsService _storeSettingsService;

    public SalesController(ISalesService salesService, IStoreSettingsService storeSettingsService)
    {
        _salesService = salesService;
        _storeSettingsService = storeSettingsService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var store = await _storeSettingsService.GetDisplayAsync(cancellationToken);
        var storeJson = JsonSerializer.Serialize(
            store,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

        return View(new PosViewModel
        {
            Store = store,
            StoreJson = storeJson
        });
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
