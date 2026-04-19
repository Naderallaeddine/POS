using Microsoft.AspNetCore.Mvc.Rendering;
using POS.DTOs.Categories;
using POS.ViewModels.Categories;

namespace POS.Interfaces.Services;

public interface ICategoryService
{
    Task<CategoryListViewModel> GetListAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SelectListItem>> GetSelectListAsync(CancellationToken cancellationToken = default);

    Task<CategoryEditViewModel?> GetEditModelAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(CategoryUpsertDto dto, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, CategoryUpsertDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

