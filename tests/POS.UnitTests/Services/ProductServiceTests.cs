using POS.DTOs.Categories;
using POS.DTOs.Products;
using POS.Infrastructure.Data.Seed;
using POS.Services;
using POS.UnitTests.Db;

namespace POS.UnitTests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_Throws_when_category_invalid()
    {
        await using var tdb = new SqliteTestDb();
        var categoryService = new CategoryService(tdb.Db);
        var productService = new ProductService(tdb.Db, categoryService);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            productService.CreateAsync(new ProductUpsertDto
            {
                Name = "Milk",
                Barcode = "MILK-001",
                CategoryId = Guid.NewGuid(),
                CostPrice = 1.0m,
                SalePrice = 1.5m,
                StockQuantity = 5m,
                IsActive = true
            }));

        Assert.Contains("Invalid category", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_Throws_when_barcode_duplicate_in_branch()
    {
        await using var tdb = new SqliteTestDb();
        var categoryService = new CategoryService(tdb.Db);
        var productService = new ProductService(tdb.Db, categoryService);

        var categoryId = await categoryService.CreateAsync(new CategoryUpsertDto { Name = "Dairy", Description = null });

        await productService.CreateAsync(new ProductUpsertDto
        {
            Name = "Milk",
            Barcode = "DUP-001",
            CategoryId = categoryId,
            CostPrice = 1.0m,
            SalePrice = 1.5m,
            StockQuantity = 5m,
            IsActive = true
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            productService.CreateAsync(new ProductUpsertDto
            {
                Name = "Another Milk",
                Barcode = "DUP-001",
                CategoryId = categoryId,
                CostPrice = 1.1m,
                SalePrice = 1.6m,
                StockQuantity = 2m,
                IsActive = true
            }));

        Assert.Contains("same barcode", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateAsync_Returns_false_when_product_missing()
    {
        await using var tdb = new SqliteTestDb();
        var categoryService = new CategoryService(tdb.Db);
        var productService = new ProductService(tdb.Db, categoryService);

        var categoryId = await categoryService.CreateAsync(new CategoryUpsertDto { Name = "Misc", Description = null });

        var ok = await productService.UpdateAsync(Guid.NewGuid(), new ProductUpsertDto
        {
            Name = "Does not exist",
            Barcode = null,
            CategoryId = categoryId,
            CostPrice = 1.0m,
            SalePrice = 2.0m,
            StockQuantity = 0m,
            IsActive = true
        });

        Assert.False(ok);
    }
}

