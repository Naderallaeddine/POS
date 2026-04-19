using POS.DTOs.Customers;
using POS.ViewModels.Customers;

namespace POS.Interfaces.Services;

public interface ICustomerService
{
    Task<CustomerListViewModel> GetListAsync(string? query, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<CustomerEditViewModel?> GetEditModelAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(CustomerUpsertDto dto, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, CustomerUpsertDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

