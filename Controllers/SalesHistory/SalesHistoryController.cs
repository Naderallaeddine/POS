using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.Interfaces.Services;

namespace POS.Controllers.SalesHistory;

[Authorize(Policy = "RequireSalesAccess")]
public class SalesHistoryController : Controller
{
    private readonly ISalesHistoryService _salesHistoryService;

    public SalesHistoryController(ISalesHistoryService salesHistoryService)
    {
        _salesHistoryService = salesHistoryService;
    }

    public async Task<IActionResult> Index(
        string? q,
        DateTimeOffset? dateFrom,
        DateTimeOffset? dateTo,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var vm = await _salesHistoryService.GetListAsync(q, dateFrom, dateTo, pageNumber, pageSize, cancellationToken);
        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _salesHistoryService.GetDetailsAsync(id, cancellationToken);
        if (vm is null)
        {
            return NotFound();
        }

        return View(vm);
    }
}

