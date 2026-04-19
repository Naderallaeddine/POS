using Microsoft.EntityFrameworkCore;
using POS.Infrastructure.Data;
using POS.Infrastructure.Data.Seed;
using POS.Interfaces.Services;
using POS.ViewModels.Common;
using POS.ViewModels.SalesHistory;

namespace POS.Services;

public class SalesHistoryService : ISalesHistoryService
{
    private readonly ApplicationDbContext _db;

    public SalesHistoryService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<SalesListViewModel> GetListAsync(
        string? query,
        DateTimeOffset? dateFrom,
        DateTimeOffset? dateTo,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;
        query = string.IsNullOrWhiteSpace(query) ? null : query.Trim();

        IQueryable<Domain.Entities.Sale> baseQuery = _db.Sales
            .AsNoTracking()
            .Where(s => s.BranchId == BranchSeed.DefaultBranchId)
            .Include(s => s.Customer);

        if (dateFrom is not null)
        {
            baseQuery = baseQuery.Where(s => s.SaleDate >= dateFrom.Value);
        }

        if (dateTo is not null)
        {
            baseQuery = baseQuery.Where(s => s.SaleDate <= dateTo.Value);
        }

        if (query is not null)
        {
            baseQuery = baseQuery.Where(s =>
                s.ReceiptNumber.Contains(query) ||
                (s.Customer != null && s.Customer.FullName.Contains(query)));
        }

        var total = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderByDescending(s => s.SaleDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SalesListItemVm
            {
                Id = s.Id,
                SaleDate = s.SaleDate,
                ReceiptNumber = s.ReceiptNumber,
                Customer = s.Customer != null ? s.Customer.FullName : "Walk-in",
                PaymentMethod = s.PaymentMethod.ToString(),
                Total = s.Total
            })
            .ToListAsync(cancellationToken);

        return new SalesListViewModel
        {
            Query = query,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Results = new PagedResultViewModel<SalesListItemVm>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total
            }
        };
    }

    public async Task<SaleDetailsViewModel?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _db.Sales
            .AsNoTracking()
            .Where(s => s.Id == id && s.BranchId == BranchSeed.DefaultBranchId)
            .Include(s => s.Customer)
            .Include(s => s.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(cancellationToken);

        if (sale is null)
        {
            return null;
        }

        return new SaleDetailsViewModel
        {
            Id = sale.Id,
            SaleDate = sale.SaleDate,
            ReceiptNumber = sale.ReceiptNumber,
            Customer = sale.Customer != null ? sale.Customer.FullName : "Walk-in",
            PaymentMethod = sale.PaymentMethod.ToString(),
            Subtotal = sale.Subtotal,
            DiscountAmount = sale.DiscountAmount,
            TaxAmount = sale.TaxAmount,
            Total = sale.Total,
            PaidAmount = sale.PaidAmount,
            ChangeAmount = sale.ChangeAmount,
            Items = sale.Items
                .OrderBy(i => i.Product.Name)
                .Select(i => new SaleDetailsItemVm
                {
                    ProductName = i.Product.Name,
                    Barcode = i.Product.Barcode,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal
                })
                .ToList()
        };
    }
}

