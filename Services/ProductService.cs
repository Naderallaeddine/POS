using Microsoft.EntityFrameworkCore;
using POS.DTOs.Products;
using POS.Domain.Entities;
using POS.Infrastructure.Data;
using POS.Infrastructure.Data.Seed;
using POS.Interfaces.Services;
using POS.ViewModels.Products;

namespace POS.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _db;
    private readonly ICategoryService _categoryService;

    public ProductService(ApplicationDbContext db, ICategoryService categoryService)
    {
        _db = db;
        _categoryService = categoryService;
    }

    public async Task<ProductListViewModel> GetListAsync(CancellationToken cancellationToken = default)
    {
        var products = await _db.Products
            .AsNoTracking()
            .Where(p => p.BranchId == BranchSeed.DefaultBranchId)
            .OrderBy(p => p.Name)
            .Select(p => new ProductListItemVm
            {
                Id = p.Id,
                Name = p.Name,
                Barcode = p.Barcode,
                CategoryName = p.Category.Name,
                CostPrice = p.CostPrice,
                SalePrice = p.SellingPrice,
                StockQuantity = p.CurrentStock,
                IsActive = p.IsActive
            })
            .ToListAsync(cancellationToken);

        return new ProductListViewModel { Products = products };
    }

    public async Task<ProductEditViewModel> GetCreateModelAsync(CancellationToken cancellationToken = default)
    {
        return new ProductEditViewModel
        {
            Categories = await _categoryService.GetSelectListAsync(cancellationToken),
            IsActive = true
        };
    }

    public async Task<ProductEditViewModel?> GetEditModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var vm = await _db.Products
            .AsNoTracking()
            .Where(p => p.Id == id && p.BranchId == BranchSeed.DefaultBranchId)
            .Select(p => new ProductEditViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Barcode = p.Barcode,
                CategoryId = p.CategoryId,
                CostPrice = p.CostPrice,
                SalePrice = p.SellingPrice,
                StockQuantity = p.CurrentStock,
                IsActive = p.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (vm is null)
        {
            return null;
        }

        vm.Categories = await _categoryService.GetSelectListAsync(cancellationToken);
        return vm;
    }

    public async Task<Guid> CreateAsync(ProductUpsertDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureCategoryIsValidAsync(dto.CategoryId, cancellationToken);

        if (!string.IsNullOrWhiteSpace(dto.Barcode))
        {
            var barcodeExists = await _db.Products.AnyAsync(
                p => p.BranchId == BranchSeed.DefaultBranchId && p.Barcode == dto.Barcode,
                cancellationToken);

            if (barcodeExists)
            {
                throw new InvalidOperationException("A product with the same barcode already exists.");
            }
        }

        var entity = new Product
        {
            Id = Guid.NewGuid(),
            BranchId = BranchSeed.DefaultBranchId,
            CategoryId = dto.CategoryId,
            Name = dto.Name.Trim(),
            Barcode = string.IsNullOrWhiteSpace(dto.Barcode) ? null : dto.Barcode.Trim(),
            CostPrice = dto.CostPrice,
            SellingPrice = dto.SalePrice,
            CurrentStock = dto.StockQuantity,
            IsActive = dto.IsActive
        };

        _db.Products.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, ProductUpsertDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureCategoryIsValidAsync(dto.CategoryId, cancellationToken);

        var entity = await _db.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.BranchId == BranchSeed.DefaultBranchId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(dto.Barcode))
        {
            var barcodeExists = await _db.Products.AnyAsync(
                p => p.BranchId == BranchSeed.DefaultBranchId && p.Id != id && p.Barcode == dto.Barcode,
                cancellationToken);

            if (barcodeExists)
            {
                throw new InvalidOperationException("A product with the same barcode already exists.");
            }
        }

        entity.Name = dto.Name.Trim();
        entity.Barcode = string.IsNullOrWhiteSpace(dto.Barcode) ? null : dto.Barcode.Trim();
        entity.CategoryId = dto.CategoryId;
        entity.CostPrice = dto.CostPrice;
        entity.SellingPrice = dto.SalePrice;
        entity.CurrentStock = dto.StockQuantity;
        entity.IsActive = dto.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.BranchId == BranchSeed.DefaultBranchId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        _db.Products.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureCategoryIsValidAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        var exists = await _db.Categories.AnyAsync(
            c => c.Id == categoryId && c.BranchId == BranchSeed.DefaultBranchId,
            cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException("Invalid category selected.");
        }
    }
}

