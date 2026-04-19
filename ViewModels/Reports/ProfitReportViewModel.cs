namespace POS.ViewModels.Reports;

public class ProfitReportViewModel
{
    public DateRangeFilterViewModel Filter { get; set; } = new();

    public decimal SalesTotal { get; set; }
    public decimal EstimatedCogsTotal { get; set; }
    public decimal GrossProfit => SalesTotal - EstimatedCogsTotal;

    public decimal ExpensesTotal { get; set; }
    public decimal NetProfit => GrossProfit - ExpensesTotal;

    public IReadOnlyList<ProfitByDayRowVm> Rows { get; set; } = Array.Empty<ProfitByDayRowVm>();
}

public class ProfitByDayRowVm
{
    public DateOnly Date { get; set; }
    public decimal SalesTotal { get; set; }
    public decimal EstimatedCogsTotal { get; set; }
    public decimal GrossProfit => SalesTotal - EstimatedCogsTotal;
    public decimal ExpensesTotal { get; set; }
    public decimal NetProfit => GrossProfit - ExpensesTotal;
}

