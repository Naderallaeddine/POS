namespace POS.ViewModels.Reports;

public class ExpenseReportViewModel
{
    public DateRangeFilterViewModel Filter { get; set; } = new();

    public decimal TotalExpenses { get; set; }
    public int ExpensesCount { get; set; }

    public IReadOnlyList<ExpenseRowVm> Rows { get; set; } = Array.Empty<ExpenseRowVm>();
}

public class ExpenseRowVm
{
    public DateTimeOffset Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal Amount { get; set; }
}

