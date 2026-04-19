using POS.ViewModels.Reports;

namespace POS.Interfaces.Services;

public interface IReportsService
{
    Task<DailySalesReportViewModel> GetDailySalesAsync(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken = default);

    Task<MonthlySalesReportViewModel> GetMonthlySalesAsync(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken = default);

    Task<StockReportViewModel> GetStockAsync(CancellationToken cancellationToken = default);

    Task<ExpenseReportViewModel> GetExpensesAsync(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken = default);

    Task<ProfitReportViewModel> GetProfitAsync(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken = default);
}

