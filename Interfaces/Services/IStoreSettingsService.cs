using POS.DTOs.Settings;
using POS.ViewModels.Settings;

namespace POS.Interfaces.Services;

public interface IStoreSettingsService
{
    Task<StoreSettingsDisplayViewModel> GetDisplayAsync(CancellationToken cancellationToken = default);

    Task<StoreSettingsEditViewModel> GetForEditAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(StoreSettingsUpdateDto dto, CancellationToken cancellationToken = default);
}
