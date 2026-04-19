using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.DTOs.Settings;
using POS.Infrastructure.Data;
using POS.Infrastructure.Data.Seed;
using POS.Interfaces.Services;
using POS.ViewModels.Settings;

namespace POS.Services;

public class StoreSettingsService : IStoreSettingsService
{
    private readonly ApplicationDbContext _db;

    public StoreSettingsService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<StoreSettingsDisplayViewModel> GetDisplayAsync(CancellationToken cancellationToken = default)
    {
        var entity = await _db.StoreSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.BranchId == BranchSeed.DefaultBranchId, cancellationToken);

        return entity is null ? Defaults() : MapDisplay(entity);
    }

    public async Task<StoreSettingsEditViewModel> GetForEditAsync(CancellationToken cancellationToken = default)
    {
        var entity = await _db.StoreSettings
            .FirstOrDefaultAsync(s => s.BranchId == BranchSeed.DefaultBranchId, cancellationToken);

        if (entity is null)
        {
            return MapEdit(Defaults());
        }

        return MapEdit(MapDisplay(entity));
    }

    public async Task UpdateAsync(StoreSettingsUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.StoreSettings
            .FirstOrDefaultAsync(s => s.BranchId == BranchSeed.DefaultBranchId, cancellationToken);

        if (entity is null)
        {
            entity = new StoreSettings
            {
                Id = Guid.NewGuid(),
                BranchId = BranchSeed.DefaultBranchId
            };
            _db.StoreSettings.Add(entity);
        }

        entity.StoreName = dto.StoreName.Trim();
        entity.StoreAddress = string.IsNullOrWhiteSpace(dto.StoreAddress) ? null : dto.StoreAddress.Trim();
        entity.StorePhone = string.IsNullOrWhiteSpace(dto.StorePhone) ? null : dto.StorePhone.Trim();
        entity.StoreEmail = string.IsNullOrWhiteSpace(dto.StoreEmail) ? null : dto.StoreEmail.Trim();
        entity.TaxRatePercent = dto.TaxRatePercent;
        entity.ReceiptFooter = string.IsNullOrWhiteSpace(dto.ReceiptFooter) ? null : dto.ReceiptFooter.Trim();
        entity.CurrencyCode = dto.CurrencyCode.Trim().ToUpperInvariant();
        entity.CurrencySymbol = string.IsNullOrWhiteSpace(dto.CurrencySymbol) ? null : dto.CurrencySymbol.Trim();

        await _db.SaveChangesAsync(cancellationToken);
    }

    private static StoreSettingsDisplayViewModel Defaults() => new()
    {
        StoreName = "POS Store",
        TaxRatePercent = 0m,
        ReceiptFooter = "Thank you for your purchase.",
        CurrencyCode = "USD",
        CurrencySymbol = "$"
    };

    private static StoreSettingsDisplayViewModel MapDisplay(StoreSettings s) => new()
    {
        StoreName = s.StoreName,
        StoreAddress = s.StoreAddress,
        StorePhone = s.StorePhone,
        StoreEmail = s.StoreEmail,
        TaxRatePercent = s.TaxRatePercent,
        ReceiptFooter = s.ReceiptFooter,
        CurrencyCode = s.CurrencyCode,
        CurrencySymbol = s.CurrencySymbol
    };

    private static StoreSettingsEditViewModel MapEdit(StoreSettingsDisplayViewModel s) => new()
    {
        StoreName = s.StoreName,
        StoreAddress = s.StoreAddress,
        StorePhone = s.StorePhone,
        StoreEmail = s.StoreEmail,
        TaxRatePercent = s.TaxRatePercent,
        ReceiptFooter = s.ReceiptFooter,
        CurrencyCode = s.CurrencyCode,
        CurrencySymbol = s.CurrencySymbol
    };
}
