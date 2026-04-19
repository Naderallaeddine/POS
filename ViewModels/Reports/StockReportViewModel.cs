namespace POS.ViewModels.Reports;

public class StockReportViewModel
{
    public IReadOnlyList<StockRowVm> Rows { get; set; } = Array.Empty<StockRowVm>();

    public int ProductsCount { get; set; }
    public int LowStockCount { get; set; }
}

public class StockRowVm
{
    public string ProductName { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal StockQuantity { get; set; }
    public decimal ReorderLevel { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsActive { get; set; }
}

