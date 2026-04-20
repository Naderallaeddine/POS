using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.DTOs.Categories;
using POS.DTOs.Sales;
using POS.Infrastructure.Data.Seed;
using POS.Services;
using POS.UnitTests.Db;

namespace POS.UnitTests.Services;

public class SalesServiceTests
{
    [Fact]
    public async Task CreateSaleAsync_Deducts_stock_and_creates_movements()
    {
        await using var tdb = new SqliteTestDb();
        var categoryService = new CategoryService(tdb.Db);
        var categoryId = await categoryService.CreateAsync(new CategoryUpsertDto { Name = "Test", Description = null });

        var productId = Guid.NewGuid();
        tdb.Db.Products.Add(new Product
        {
            Id = productId,
            BranchId = BranchSeed.DefaultBranchId,
            CategoryId = categoryId,
            Name = "Widget",
            Barcode = "W-1",
            CostPrice = 5m,
            SellingPrice = 10m,
            CurrentStock = 10m,
            IsActive = true
        });
        await tdb.Db.SaveChangesAsync();

        var svc = new SalesService(tdb.Db);

        var saleId = await svc.CreateSaleAsync(new SaleCreateDto
        {
            CustomerId = null,
            PaymentMethod = PaymentMethod.Cash,
            DiscountAmount = 0m,
            TaxAmount = 0m,
            PaidAmount = 50m,
            Items =
            {
                new SaleCreateItemDto { ProductId = productId, Quantity = 2m }
            }
        });

        var product = tdb.Db.Products.Single(p => p.Id == productId);
        Assert.Equal(8m, product.CurrentStock);

        Assert.Single(tdb.Db.Sales.Where(s => s.Id == saleId));
        Assert.Single(tdb.Db.SaleItems.Where(i => i.SaleId == saleId));
        Assert.Single(tdb.Db.StockMovements.Where(m => m.SaleId == saleId && m.ProductId == productId));
    }

    [Fact]
    public async Task CreateSaleAsync_Throws_when_insufficient_stock()
    {
        await using var tdb = new SqliteTestDb();
        var categoryService = new CategoryService(tdb.Db);
        var categoryId = await categoryService.CreateAsync(new CategoryUpsertDto { Name = "Test", Description = null });

        var productId = Guid.NewGuid();
        tdb.Db.Products.Add(new Product
        {
            Id = productId,
            BranchId = BranchSeed.DefaultBranchId,
            CategoryId = categoryId,
            Name = "LowStock",
            Barcode = "LS-1",
            CostPrice = 1m,
            SellingPrice = 2m,
            CurrentStock = 1m,
            IsActive = true
        });
        await tdb.Db.SaveChangesAsync();

        var svc = new SalesService(tdb.Db);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.CreateSaleAsync(new SaleCreateDto
            {
                CustomerId = null,
                PaymentMethod = PaymentMethod.Cash,
                DiscountAmount = 0m,
                TaxAmount = 0m,
                PaidAmount = 10m,
                Items =
                {
                    new SaleCreateItemDto { ProductId = productId, Quantity = 2m }
                }
            }));

        Assert.Contains("Insufficient stock", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}

