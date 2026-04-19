using POS.DTOs.Suppliers;
using POS.ViewModels.Suppliers;

namespace POS.Interfaces.Services;

public interface ISupplierService
{
    Task<SupplierListViewModel> GetListAsync(string? query, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<SupplierEditViewModel?> GetEditModelAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(SupplierUpsertDto dto, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, SupplierUpsertDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

