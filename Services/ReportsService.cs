using Microsoft.EntityFrameworkCore;
using POS.Infrastructure.Data;
using POS.Infrastructure.Data.Seed;
using POS.Interfaces.Services;
using POS.ViewModels.Reports;

namespace POS.Services;

public class ReportsService : IReportsService
{
    private readonly ApplicationDbContext _db;

    public ReportsService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<DailySalesReportViewModel> GetDailySalesAsync(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken = default)
    {
        var (from, to) = NormalizeRange(dateFrom, dateTo);

        IQueryable<Domain.Entities.Sale> q = _db.Sales.AsNoTracking().Where(s => s.BranchId == BranchSeed.DefaultBranchId);
        q = ApplyRange(q, from, to);

        var rows = await q
            .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month, s.SaleDate.Day })
            .Select(g => new DailySalesRowVm
            {
                Date = new DateOnly(g.Key.Year, g.Key.Month, g.Key.Day),
                InvoicesCount = g.Count(),
                TotalSales = g.Sum(x => x.Total)
            })
            .OrderBy(r => r.Date)
            .ToListAsync(cancellationToken);

        return new DailySalesReportViewModel
        {
            Filter = new DateRangeFilterViewModel { DateFrom = from, DateTo = to },
            TotalSales = rows.Sum(r => r.TotalSales),
            InvoicesCount = rows.Sum(r => r.InvoicesCount),
            Rows = rows
        };
    }

    public async Task<MonthlySalesReportViewModel> GetMonthlySalesAsync(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken = default)
    {
        var (from, to) = NormalizeRange(dateFrom, dateTo);

        IQueryable<Domain.Entities.Sale> q = _db.Sales.AsNoTracking().Where(s => s.BranchId == BranchSeed.DefaultBranchId);
        q = ApplyRange(q, from, to);

        var rows = await q
            .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
            .Select(g => new MonthlySalesRowVm
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                InvoicesCount = g.Count(),
                TotalSales = g.Sum(x => x.Total)
            })
            .OrderBy(r => r.Year)
            .ThenBy(r => r.Month)
            .ToListAsync(cancellationToken);

        return new MonthlySalesReportViewModel
        {
            Filter = new DateRangeFilterViewModel { DateFrom = from, DateTo = to },
            TotalSales = rows.Sum(r => r.TotalSales),
            InvoicesCount = rows.Sum(r => r.InvoicesCount),
            Rows = rows
        };
    }

    public async Task<StockReportViewModel> GetStockAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _db.Products
            .AsNoTracking()
            .Where(p => p.BranchId == BranchSeed.DefaultBranchId)
            .OrderBy(p => p.Name)
            .Select(p => new StockRowVm
            {
                ProductName = p.Name,
                Barcode = p.Barcode,
                CategoryName = p.Category.Name,
                StockQuantity = p.CurrentStock,
                ReorderLevel = p.ReorderLevel,
                IsLowStock = p.IsActive && p.CurrentStock <= (p.ReorderLevel == 0 ? 10 : p.ReorderLevel),
                IsActive = p.IsActive
            })
            .ToListAsync(cancellationToken);

        return new StockReportViewModel
        {
            Rows = rows,
            ProductsCount = rows.Count,
            LowStockCount = rows.Count(r => r.IsLowStock)
        };
    }

    public async Task<ExpenseReportViewModel> GetExpensesAsync(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken = default)
    {
        var (from, to) = NormalizeRange(dateFrom, dateTo);

        IQueryable<Domain.Entities.Expense> q = _db.Expenses.AsNoTracking().Where(e => e.BranchId == BranchSeed.DefaultBranchId);
        if (from is not null)
        {
            q = q.Where(e => e.ExpenseDate >= from.Value);
        }

        if (to is not null)
        {
            q = q.Where(e => e.ExpenseDate <= to.Value);
        }

        var rows = await q
            .OrderByDescending(e => e.ExpenseDate)
            .Select(e => new ExpenseRowVm
            {
                Date = e.ExpenseDate,
                Title = e.Title,
                Category = e.Category,
                Amount = e.Amount
            })
            .ToListAsync(cancellationToken);

        return new ExpenseReportViewModel
        {
            Filter = new DateRangeFilterViewModel { DateFrom = from, DateTo = to },
            TotalExpenses = rows.Sum(r => r.Amount),
            ExpensesCount = rows.Count,
            Rows = rows
        };
    }

    public async Task<ProfitReportViewModel> GetProfitAsync(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken = default)
    {
        var (from, to) = NormalizeRange(dateFrom, dateTo);

        // Note: This estimates COGS using current Product.CostPrice (historical cost is not stored per sale item yet).
        IQueryable<Domain.Entities.SaleItem> itemsQ = _db.SaleItems
            .AsNoTracking()
            .Where(i => i.BranchId == BranchSeed.DefaultBranchId)
            .Include(i => i.Sale)
            .Include(i => i.Product);

        if (from is not null)
        {
            itemsQ = itemsQ.Where(i => i.Sale.SaleDate >= from.Value);
        }

        if (to is not null)
        {
            itemsQ = itemsQ.Where(i => i.Sale.SaleDate <= to.Value);
        }

        var salesByDay = await itemsQ
            .GroupBy(i => new { i.Sale.SaleDate.Year, i.Sale.SaleDate.Month, i.Sale.SaleDate.Day })
            .Select(g => new
            {
                Date = new DateOnly(g.Key.Year, g.Key.Month, g.Key.Day),
                SalesTotal = g.Sum(x => x.LineTotal),
                EstimatedCogsTotal = g.Sum(x => x.Quantity * x.Product.CostPrice)
            })
            .ToListAsync(cancellationToken);

        IQueryable<Domain.Entities.Expense> expenseQ = _db.Expenses.AsNoTracking().Where(e => e.BranchId == BranchSeed.DefaultBranchId);
        if (from is not null)
        {
            expenseQ = expenseQ.Where(e => e.ExpenseDate >= from.Value);
        }

        if (to is not null)
        {
            expenseQ = expenseQ.Where(e => e.ExpenseDate <= to.Value);
        }

        var expensesByDay = await expenseQ
            .GroupBy(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month, e.ExpenseDate.Day })
            .Select(g => new
            {
                Date = new DateOnly(g.Key.Year, g.Key.Month, g.Key.Day),
                ExpensesTotal = g.Sum(x => x.Amount)
            })
            .ToListAsync(cancellationToken);

        var expenseMap = expensesByDay.ToDictionary(x => x.Date, x => x.ExpensesTotal);

        var rows = salesByDay
            .OrderBy(x => x.Date)
            .Select(x => new ProfitByDayRowVm
            {
                Date = x.Date,
                SalesTotal = x.SalesTotal,
                EstimatedCogsTotal = x.EstimatedCogsTotal,
                ExpensesTotal = expenseMap.TryGetValue(x.Date, out var ex) ? ex : 0m
            })
            .ToList();

        return new ProfitReportViewModel
        {
            Filter = new DateRangeFilterViewModel { DateFrom = from, DateTo = to },
            SalesTotal = rows.Sum(r => r.SalesTotal),
            EstimatedCogsTotal = rows.Sum(r => r.EstimatedCogsTotal),
            ExpensesTotal = rows.Sum(r => r.ExpensesTotal),
            Rows = rows
        };
    }

    private static (DateTimeOffset? From, DateTimeOffset? To) NormalizeRange(DateTimeOffset? from, DateTimeOffset? to)
    {
        if (from is null && to is null)
        {
            return (null, null);
        }

        if (from is not null && to is not null && from > to)
        {
            (from, to) = (to, from);
        }

        return (from, to);
    }

    private static IQueryable<Domain.Entities.Sale> ApplyRange(
        IQueryable<Domain.Entities.Sale> q,
        DateTimeOffset? from,
        DateTimeOffset? to)
    {
        if (from is not null)
        {
            q = q.Where(s => s.SaleDate >= from.Value);
        }

        if (to is not null)
        {
            q = q.Where(s => s.SaleDate <= to.Value);
        }

        return q;
    }
}

