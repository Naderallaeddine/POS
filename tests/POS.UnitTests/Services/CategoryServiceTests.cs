using POS.Domain.Entities;
using POS.DTOs.Categories;
using POS.Infrastructure.Data.Seed;
using POS.Services;
using POS.UnitTests.Db;

namespace POS.UnitTests.Services;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_Throws_when_name_duplicate_in_branch()
    {
        await using var tdb = new SqliteTestDb();
        var svc = new CategoryService(tdb.Db);

        await svc.CreateAsync(new CategoryUpsertDto
        {
            Name = "Beverages",
            Description = null
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.CreateAsync(new CategoryUpsertDto { Name = "Beverages", Description = null }));

        Assert.Contains("same name", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteAsync_Throws_when_category_has_products()
    {
        await using var tdb = new SqliteTestDb();
        var svc = new CategoryService(tdb.Db);

        var categoryId = await svc.CreateAsync(new CategoryUpsertDto
        {
            Name = "Snacks",
            Description = null
        });

        tdb.Db.Products.Add(new Product
        {
            Id = Guid.NewGuid(),
            BranchId = BranchSeed.DefaultBranchId,
            CategoryId = categoryId,
            Name = "Chips",
            Barcode = "123",
            CostPrice = 1.00m,
            SellingPrice = 2.00m,
            CurrentStock = 10m,
            IsActive = true
        });
        await tdb.Db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.DeleteAsync(categoryId));
        Assert.Contains("has products", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}

