using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.DTOs.Settings;
using POS.Interfaces.Services;
using POS.ViewModels.Settings;

namespace POS.Controllers.Settings;

[Authorize(Policy = "RequireAdministrator")]
public class SettingsController : Controller
{
    private readonly IStoreSettingsService _storeSettingsService;

    public SettingsController(IStoreSettingsService storeSettingsService)
    {
        _storeSettingsService = storeSettingsService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Settings";
        var vm = await _storeSettingsService.GetForEditAsync(cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(StoreSettingsEditViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Settings";
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _storeSettingsService.UpdateAsync(new StoreSettingsUpdateDto
        {
            StoreName = model.StoreName,
            StoreAddress = model.StoreAddress,
            StorePhone = model.StorePhone,
            StoreEmail = model.StoreEmail,
            TaxRatePercent = model.TaxRatePercent,
            ReceiptFooter = model.ReceiptFooter,
            CurrencyCode = model.CurrencyCode,
            CurrencySymbol = model.CurrencySymbol
        }, cancellationToken);

        TempData["SuccessMessage"] = "Settings saved.";
        return RedirectToAction(nameof(Index));
    }
}
