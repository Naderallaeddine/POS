namespace POS.ViewModels.Dashboard;

public class DashboardViewModel
{
    public decimal TodaysSalesTotal { get; set; }

    public int ProductsCount { get; set; }

    public int CustomersCount { get; set; }

    public int LowStockThreshold { get; set; }

    public IReadOnlyList<LowStockProductVm> LowStockProducts { get; set; } = Array.Empty<LowStockProductVm>();

    public IReadOnlyList<RecentSaleVm> RecentSales { get; set; } = Array.Empty<RecentSaleVm>();
}

public class LowStockProductVm
{
    public Guid ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Sku { get; set; }

    public decimal CurrentStock { get; set; }

    public decimal ReorderLevel { get; set; }
}

public class RecentSaleVm
{
    public Guid SaleId { get; set; }

    public DateTimeOffset SaleDate { get; set; }

    public string ReceiptNumber { get; set; } = string.Empty;

    public string? CustomerName { get; set; }

    public decimal Total { get; set; }
}

