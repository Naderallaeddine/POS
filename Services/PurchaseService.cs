using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.DTOs.Purchases;
using POS.Infrastructure.Data;
using POS.Infrastructure.Data.Seed;
using POS.Interfaces.Services;
using POS.ViewModels.Purchases;

namespace POS.Services;

public class PurchaseService : IPurchaseService
{
    private readonly ApplicationDbContext _db;

    public PurchaseService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PurchaseCreateViewModel> GetCreateModelAsync(CancellationToken cancellationToken = default)
    {
        var suppliers = await _db.Suppliers
            .AsNoTracking()
            .Where(s => s.BranchId == BranchSeed.DefaultBranchId)
            .OrderBy(s => s.Name)
            .Select(s => new SelectListItem(s.Name, s.Id.ToString()))
            .ToListAsync(cancellationToken);

        var products = await _db.Products
            .AsNoTracking()
            .Where(p => p.BranchId == BranchSeed.DefaultBranchId && p.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => new SelectListItem(p.Name, p.Id.ToString()))
            .ToListAsync(cancellationToken);

        return new PurchaseCreateViewModel
        {
            Suppliers = suppliers,
            Products = products,
            PurchaseDate = DateTimeOffset.UtcNow
        };
    }

    public async Task<Guid> CreateAsync(PurchaseCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Items.Count == 0)
        {
            throw new InvalidOperationException("Add at least one item.");
        }

        dto.ReferenceNumber = dto.ReferenceNumber.Trim();
        if (string.IsNullOrWhiteSpace(dto.ReferenceNumber))
        {
            throw new InvalidOperationException("Reference number is required.");
        }

        var supplierExists = await _db.Suppliers.AnyAsync(
            s => s.Id == dto.SupplierId && s.BranchId == BranchSeed.DefaultBranchId,
            cancellationToken);

        if (!supplierExists)
        {
            throw new InvalidOperationException("Invalid supplier selected.");
        }

        var referenceExists = await _db.Purchases.AnyAsync(
            p => p.BranchId == BranchSeed.DefaultBranchId && p.ReferenceNumber == dto.ReferenceNumber,
            cancellationToken);

        if (referenceExists)
        {
            throw new InvalidOperationException("A purchase with the same reference number already exists.");
        }

        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => p.BranchId == BranchSeed.DefaultBranchId && productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        if (products.Count != productIds.Count)
        {
            throw new InvalidOperationException("One or more products are invalid.");
        }

        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

        var purchaseId = Guid.NewGuid();
        var purchase = new Purchase
        {
            Id = purchaseId,
            BranchId = BranchSeed.DefaultBranchId,
            SupplierId = dto.SupplierId,
            PurchaseDate = dto.PurchaseDate,
            ReferenceNumber = dto.ReferenceNumber,
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };

        decimal subtotal = 0m;
        decimal discountTotal = 0m;
        decimal taxTotal = 0m;

        var items = new List<PurchaseItem>(dto.Items.Count);
        var movements = new List<StockMovement>(dto.Items.Count);

        foreach (var i in dto.Items)
        {
            if (i.Quantity <= 0)
            {
                throw new InvalidOperationException("Quantity must be greater than 0.");
            }

            var unitCost = i.UnitCost;
            var lineBase = unitCost * i.Quantity;
            var lineTotal = lineBase - i.DiscountAmount + i.TaxAmount;
            if (lineTotal < 0)
            {
                throw new InvalidOperationException("Line total cannot be negative.");
            }

            subtotal += lineBase;
            discountTotal += i.DiscountAmount;
            taxTotal += i.TaxAmount;

            items.Add(new PurchaseItem
            {
                Id = Guid.NewGuid(),
                BranchId = BranchSeed.DefaultBranchId,
                PurchaseId = purchaseId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitCost = unitCost,
                DiscountAmount = i.DiscountAmount,
                TaxAmount = i.TaxAmount,
                LineTotal = lineTotal
            });

            products[i.ProductId].CurrentStock += i.Quantity;

            movements.Add(new StockMovement
            {
                Id = Guid.NewGuid(),
                BranchId = BranchSeed.DefaultBranchId,
                ProductId = i.ProductId,
                MovementDate = dto.PurchaseDate,
                Type = StockMovementType.Purchase,
                Quantity = i.Quantity,
                Reference = dto.ReferenceNumber,
                PurchaseId = purchaseId,
                Notes = "Purchase received"
            });
        }

        purchase.Subtotal = subtotal;
        purchase.DiscountAmount = discountTotal;
        purchase.TaxAmount = taxTotal;
        purchase.Total = subtotal - discountTotal + taxTotal;

        _db.Purchases.Add(purchase);
        _db.PurchaseItems.AddRange(items);
        _db.StockMovements.AddRange(movements);

        await _db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return purchaseId;
    }
}

