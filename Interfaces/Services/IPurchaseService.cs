using POS.DTOs.Purchases;
using POS.ViewModels.Purchases;

namespace POS.Interfaces.Services;

public interface IPurchaseService
{
    Task<PurchaseCreateViewModel> GetCreateModelAsync(CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(PurchaseCreateDto dto, CancellationToken cancellationToken = default);
}

