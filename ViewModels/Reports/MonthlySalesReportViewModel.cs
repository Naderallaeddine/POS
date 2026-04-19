namespace POS.ViewModels.Reports;

public class MonthlySalesReportViewModel
{
    public DateRangeFilterViewModel Filter { get; set; } = new();

    public decimal TotalSales { get; set; }
    public int InvoicesCount { get; set; }

    public IReadOnlyList<MonthlySalesRowVm> Rows { get; set; } = Array.Empty<MonthlySalesRowVm>();
}

public class MonthlySalesRowVm
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int InvoicesCount { get; set; }
    public decimal TotalSales { get; set; }
}

