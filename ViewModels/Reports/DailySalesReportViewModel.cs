namespace POS.ViewModels.Reports;

public class DailySalesReportViewModel
{
    public DateRangeFilterViewModel Filter { get; set; } = new();

    public decimal TotalSales { get; set; }
    public int InvoicesCount { get; set; }

    public IReadOnlyList<DailySalesRowVm> Rows { get; set; } = Array.Empty<DailySalesRowVm>();
}

public class DailySalesRowVm
{
    public DateOnly Date { get; set; }
    public int InvoicesCount { get; set; }
    public decimal TotalSales { get; set; }
}

