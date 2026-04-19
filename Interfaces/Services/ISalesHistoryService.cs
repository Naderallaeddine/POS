using POS.ViewModels.SalesHistory;

namespace POS.Interfaces.Services;

public interface ISalesHistoryService
{
    Task<SalesListViewModel> GetListAsync(
        string? query,
        DateTimeOffset? dateFrom,
        DateTimeOffset? dateTo,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<SaleDetailsViewModel?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}

