using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.DTOs.Categories;
using POS.Interfaces.Services;
using POS.ViewModels.Categories;

namespace POS.Controllers.Categories;

[Authorize]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var vm = await _categoryService.GetListAsync(cancellationToken);
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Add category";
        return View(new CategoryEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _categoryService.CreateAsync(new CategoryUpsertDto
            {
                Name = model.Name,
                Description = model.Description
            }, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _categoryService.GetEditModelAsync(id, cancellationToken);
        if (vm is null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Edit category";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CategoryEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var ok = await _categoryService.UpdateAsync(id, new CategoryUpsertDto
            {
                Name = model.Name,
                Description = model.Description
            }, cancellationToken);

            if (!ok)
            {
                return NotFound();
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var ok = await _categoryService.DeleteAsync(id, cancellationToken);
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

