using POS.DTOs.Sales;
using POS.ViewModels.Sales;

namespace POS.Interfaces.Services;

public interface ISalesService
{
    Task<IReadOnlyList<ProductSearchResultVm>> SearchProductsAsync(string term, int take = 20, CancellationToken cancellationToken = default);

    Task<Guid> CreateSaleAsync(SaleCreateDto dto, CancellationToken cancellationToken = default);
}

