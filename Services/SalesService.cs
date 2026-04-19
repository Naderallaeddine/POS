using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.DTOs.Sales;
using POS.Infrastructure.Data;
using POS.Infrastructure.Data.Seed;
using POS.Interfaces.Services;
using POS.ViewModels.Sales;

namespace POS.Services;

public class SalesService : ISalesService
{
    private readonly ApplicationDbContext _db;

    public SalesService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ProductSearchResultVm>> SearchProductsAsync(string term, int take = 20, CancellationToken cancellationToken = default)
    {
        term = term.Trim();
        take = take is < 1 or > 50 ? 20 : take;

        return await _db.Products
            .AsNoTracking()
            .Where(p => p.BranchId == BranchSeed.DefaultBranchId && p.IsActive)
            .Where(p =>
                p.Name.Contains(term) ||
                (p.Barcode != null && p.Barcode.Contains(term)))
            .OrderBy(p => p.Name)
            .Take(take)
            .Select(p => new ProductSearchResultVm
            {
                Id = p.Id,
                Name = p.Name,
                Barcode = p.Barcode,
                Price = p.SellingPrice,
                Stock = p.CurrentStock
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> CreateSaleAsync(SaleCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Items.Count == 0)
        {
            throw new InvalidOperationException("Add at least one item.");
        }

        // Merge duplicates & validate quantities
        var merged = dto.Items
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToList();

        if (merged.Any(i => i.Quantity <= 0))
        {
            throw new InvalidOperationException("Quantity must be greater than 0.");
        }

        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

        var productIds = merged.Select(x => x.ProductId).ToList();
        var products = await _db.Products
            .Where(p => p.BranchId == BranchSeed.DefaultBranchId && productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        if (products.Count != productIds.Count)
        {
            throw new InvalidOperationException("One or more products are invalid.");
        }

        foreach (var item in merged)
        {
            var p = products[item.ProductId];
            if (!p.IsActive)
            {
                throw new InvalidOperationException($"Product '{p.Name}' is inactive.");
            }

            if (p.CurrentStock < item.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for '{p.Name}'. Available: {p.CurrentStock}, requested: {item.Quantity}.");
            }
        }

        var receipt = await GenerateReceiptNumberAsync(cancellationToken);

        var saleId = Guid.NewGuid();
        var saleDate = DateTimeOffset.UtcNow;

        decimal subtotal = 0m;
        var items = new List<SaleItem>(merged.Count);
        var movements = new List<StockMovement>(merged.Count);

        foreach (var item in merged)
        {
            var p = products[item.ProductId];
            var unitPrice = p.SellingPrice;
            var lineBase = unitPrice * item.Quantity;

            subtotal += lineBase;

            items.Add(new SaleItem
            {
                Id = Guid.NewGuid(),
                BranchId = BranchSeed.DefaultBranchId,
                SaleId = saleId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                DiscountAmount = 0m,
                TaxAmount = 0m,
                LineTotal = lineBase
            });

            p.CurrentStock -= item.Quantity;

            movements.Add(new StockMovement
            {
                Id = Guid.NewGuid(),
                BranchId = BranchSeed.DefaultBranchId,
                ProductId = item.ProductId,
                MovementDate = saleDate,
                Type = StockMovementType.Sale,
                Quantity = -item.Quantity,
                Reference = receipt,
                SaleId = saleId,
                Notes = "POS sale"
            });
        }

        var total = subtotal - dto.DiscountAmount + dto.TaxAmount;
        if (total < 0)
        {
            throw new InvalidOperationException("Total cannot be negative.");
        }

        if (dto.PaidAmount < total)
        {
            throw new InvalidOperationException("Paid amount is less than total.");
        }

        var sale = new Sale
        {
            Id = saleId,
            BranchId = BranchSeed.DefaultBranchId,
            CustomerId = dto.CustomerId,
            SaleDate = saleDate,
            ReceiptNumber = receipt,
            PaymentMethod = dto.PaymentMethod,
            Subtotal = subtotal,
            DiscountAmount = dto.DiscountAmount,
            TaxAmount = dto.TaxAmount,
            Total = total,
            PaidAmount = dto.PaidAmount,
            ChangeAmount = dto.PaidAmount - total,
            Notes = null
        };

        _db.Sales.Add(sale);
        _db.SaleItems.AddRange(items);
        _db.StockMovements.AddRange(movements);

        await _db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return saleId;
    }

    private async Task<string> GenerateReceiptNumberAsync(CancellationToken cancellationToken)
    {
        // Simple, readable, low-collision receipt number.
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var suffix = Random.Shared.Next(1000, 9999);
            var receipt = $"S-{stamp}-{suffix}";

            var exists = await _db.Sales.AnyAsync(
                s => s.BranchId == BranchSeed.DefaultBranchId && s.ReceiptNumber == receipt,
                cancellationToken);

            if (!exists)
            {
                return receipt;
            }
        }

        // Fallback if unlikely collisions occur
        return $"S-{Guid.NewGuid():N}";
    }
}

