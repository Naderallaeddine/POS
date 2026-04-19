using POS.DTOs.Products;
using POS.ViewModels.Products;

namespace POS.Interfaces.Services;

public interface IProductService
{
    Task<ProductListViewModel> GetListAsync(CancellationToken cancellationToken = default);

    Task<ProductEditViewModel> GetCreateModelAsync(CancellationToken cancellationToken = default);

    Task<ProductEditViewModel?> GetEditModelAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(ProductUpsertDto dto, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, ProductUpsertDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

