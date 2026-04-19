using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.DTOs.Customers;
using POS.Interfaces.Services;
using POS.ViewModels.Customers;

namespace POS.Controllers.Customers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<IActionResult> Index(string? q, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var vm = await _customerService.GetListAsync(q, pageNumber, pageSize, cancellationToken);
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Add customer";
        return View(new CustomerEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _customerService.CreateAsync(new CustomerUpsertDto
        {
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _customerService.GetEditModelAsync(id, cancellationToken);
        if (vm is null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Edit customer";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CustomerEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var ok = await _customerService.UpdateAsync(id, new CustomerUpsertDto
        {
            FullName = model.FullName,
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
            var ok = await _customerService.DeleteAsync(id, cancellationToken);
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
