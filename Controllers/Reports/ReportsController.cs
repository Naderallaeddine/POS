using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.Interfaces.Services;

namespace POS.Controllers.Reports;

[Authorize]
public class ReportsController : Controller
{
    private readonly IReportsService _reportsService;

    public ReportsController(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> DailySales(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken)
    {
        var vm = await _reportsService.GetDailySalesAsync(dateFrom, dateTo, cancellationToken);
        ViewData["Title"] = "Daily sales report";
        return View(vm);
    }

    public async Task<IActionResult> MonthlySales(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken)
    {
        var vm = await _reportsService.GetMonthlySalesAsync(dateFrom, dateTo, cancellationToken);
        ViewData["Title"] = "Monthly sales report";
        return View(vm);
    }

    public async Task<IActionResult> Stock(CancellationToken cancellationToken)
    {
        var vm = await _reportsService.GetStockAsync(cancellationToken);
        ViewData["Title"] = "Stock report";
        return View(vm);
    }

    public async Task<IActionResult> Profit(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken)
    {
        var vm = await _reportsService.GetProfitAsync(dateFrom, dateTo, cancellationToken);
        ViewData["Title"] = "Profit report";
        return View(vm);
    }

    public async Task<IActionResult> Expenses(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken cancellationToken)
    {
        var vm = await _reportsService.GetExpensesAsync(dateFrom, dateTo, cancellationToken);
        ViewData["Title"] = "Expense report";
        return View(vm);
    }
}
