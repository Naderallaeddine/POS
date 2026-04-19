using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.DTOs.Products;
using POS.Interfaces.Services;
using POS.ViewModels.Products;

namespace POS.Controllers.Products;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var vm = await _productService.GetListAsync(cancellationToken);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Add product";
        return View(await _productService.GetCreateModelAsync(cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = (await _productService.GetCreateModelAsync(cancellationToken)).Categories;
            return View(model);
        }

        try
        {
            await _productService.CreateAsync(new ProductUpsertDto
            {
                Name = model.Name,
                Barcode = model.Barcode,
                CategoryId = model.CategoryId,
                CostPrice = model.CostPrice,
                SalePrice = model.SalePrice,
                StockQuantity = model.StockQuantity,
                IsActive = model.IsActive
            }, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.Categories = (await _productService.GetCreateModelAsync(cancellationToken)).Categories;
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _productService.GetEditModelAsync(id, cancellationToken);
        if (vm is null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Edit product";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProductEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var existing = await _productService.GetEditModelAsync(id, cancellationToken);
            model.Categories = existing?.Categories ?? model.Categories;
            return View(model);
        }

        try
        {
            var ok = await _productService.UpdateAsync(id, new ProductUpsertDto
            {
                Name = model.Name,
                Barcode = model.Barcode,
                CategoryId = model.CategoryId,
                CostPrice = model.CostPrice,
                SalePrice = model.SalePrice,
                StockQuantity = model.StockQuantity,
                IsActive = model.IsActive
            }, cancellationToken);

            if (!ok)
            {
                return NotFound();
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var existing = await _productService.GetEditModelAsync(id, cancellationToken);
            model.Categories = existing?.Categories ?? model.Categories;
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var ok = await _productService.DeleteAsync(id, cancellationToken);
        if (!ok)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}
