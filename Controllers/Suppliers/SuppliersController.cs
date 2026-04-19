using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.DTOs.Suppliers;
using POS.Interfaces.Services;
using POS.ViewModels.Suppliers;

namespace POS.Controllers.Suppliers;

[Authorize]
public class SuppliersController : Controller
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<IActionResult> Index(string? q, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var vm = await _supplierService.GetListAsync(q, pageNumber, pageSize, cancellationToken);
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Add supplier";
        return View(new SupplierEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _supplierService.CreateAsync(new SupplierUpsertDto
        {
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _supplierService.GetEditModelAsync(id, cancellationToken);
        if (vm is null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Edit supplier";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, SupplierEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var ok = await _supplierService.UpdateAsync(id, new SupplierUpsertDto
        {
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address
        }, cancellationToken);

        if (!ok)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var ok = await _supplierService.DeleteAsync(id, cancellationToken);
            if (!ok)
            {
                return NotFound();
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
