using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS.DTOs.Categories;
using POS.Domain.Entities;
using POS.Infrastructure.Data;
using POS.Infrastructure.Data.Seed;
using POS.Interfaces.Services;
using POS.ViewModels.Categories;

namespace POS.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _db;

    public CategoryService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CategoryListViewModel> GetListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _db.Categories
            .AsNoTracking()
            .Where(c => c.BranchId == BranchSeed.DefaultBranchId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryListItemVm
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductsCount = c.Products.Count
            })
            .ToListAsync(cancellationToken);

        return new CategoryListViewModel { Categories = items };
    }

    public async Task<IReadOnlyList<SelectListItem>> GetSelectListAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Categories
            .AsNoTracking()
            .Where(c => c.BranchId == BranchSeed.DefaultBranchId)
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryEditViewModel?> GetEditModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Categories
            .AsNoTracking()
            .Where(c => c.Id == id && c.BranchId == BranchSeed.DefaultBranchId)
            .Select(c => new CategoryEditViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(CategoryUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _db.Categories.AnyAsync(
            c => c.BranchId == BranchSeed.DefaultBranchId && c.Name == dto.Name,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("A category with the same name already exists.");
        }

        var entity = new Category
        {
            Id = Guid.NewGuid(),
            BranchId = BranchSeed.DefaultBranchId,
            Name = dto.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim()
        };

        _db.Categories.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, CategoryUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.BranchId == BranchSeed.DefaultBranchId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        var exists = await _db.Categories.AnyAsync(
            c => c.BranchId == BranchSeed.DefaultBranchId && c.Id != id && c.Name == dto.Name,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("A category with the same name already exists.");
        }

        entity.Name = dto.Name.Trim();
        entity.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id && c.BranchId == BranchSeed.DefaultBranchId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        if (entity.Products.Count > 0)
        {
            throw new InvalidOperationException("Cannot delete a category that has products.");
        }

        _db.Categories.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

