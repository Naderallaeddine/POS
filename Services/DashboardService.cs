using Microsoft.EntityFrameworkCore;
using POS.Infrastructure.Data;
using POS.Interfaces.Services;
using POS.ViewModels.Dashboard;

namespace POS.Services;

public class DashboardService : IDashboardService
{
    private const int DefaultLowStockThreshold = 10;
    private const int DefaultRecentSalesCount = 8;

    private readonly ApplicationDbContext _db;

    public DashboardService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTimeOffset.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var vm = new DashboardViewModel
        {
            LowStockThreshold = DefaultLowStockThreshold
        };

        vm.ProductsCount = await _db.Products.CountAsync(cancellationToken);
        vm.CustomersCount = await _db.Customers.CountAsync(cancellationToken);

        vm.TodaysSalesTotal = await _db.Sales
            .Where(s => s.SaleDate >= today && s.SaleDate < tomorrow)
            .SumAsync(s => (decimal?)s.Total, cancellationToken) ?? 0m;

        vm.LowStockProducts = await _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive && p.CurrentStock <= (p.ReorderLevel == 0 ? DefaultLowStockThreshold : p.ReorderLevel))
            .OrderBy(p => p.CurrentStock)
            .Select(p => new LowStockProductVm
            {
                ProductId = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                CurrentStock = p.CurrentStock,
                ReorderLevel = p.ReorderLevel == 0 ? DefaultLowStockThreshold : p.ReorderLevel
            })
            .Take(10)
            .ToListAsync(cancellationToken);

        vm.RecentSales = await _db.Sales
            .AsNoTracking()
            .OrderByDescending(s => s.SaleDate)
            .Include(s => s.Customer)
            .Select(s => new RecentSaleVm
            {
                SaleId = s.Id,
                SaleDate = s.SaleDate,
                ReceiptNumber = s.ReceiptNumber,
                CustomerName = s.Customer != null ? s.Customer.FullName : null,
                Total = s.Total
            })
            .Take(DefaultRecentSalesCount)
            .ToListAsync(cancellationToken);

        return vm;
    }
}

