using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.DTOs.Purchases;
using POS.Interfaces.Services;
using POS.ViewModels.Purchases;

namespace POS.Controllers.Purchases;

[Authorize]
public class PurchasesController : Controller
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "New purchase";
        return View(await _purchaseService.GetCreateModelAsync(cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PurchaseCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var reload = await _purchaseService.GetCreateModelAsync(cancellationToken);
            model.Suppliers = reload.Suppliers;
            model.Products = reload.Products;
            return View(model);
        }

        try
        {
            await _purchaseService.CreateAsync(new PurchaseCreateDto
            {
                SupplierId = model.SupplierId,
                PurchaseDate = model.PurchaseDate,
                ReferenceNumber = model.ReferenceNumber,
                Notes = model.Notes,
                Items = model.Items
                    .Where(i => i.ProductId != Guid.Empty && i.Quantity > 0)
                    .Select(i => new PurchaseCreateItemDto
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitCost = i.UnitCost,
                        DiscountAmount = i.DiscountAmount,
                        TaxAmount = i.TaxAmount
                    })
                    .ToList()
            }, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var reload = await _purchaseService.GetCreateModelAsync(cancellationToken);
            model.Suppliers = reload.Suppliers;
            model.Products = reload.Products;
            return View(model);
        }

        TempData["SuccessMessage"] = "Purchase saved and stock updated.";
        return RedirectToAction(nameof(Index));
    }
}
