using Microsoft.EntityFrameworkCore;
using POS.DTOs.Suppliers;
using POS.Domain.Entities;
using POS.Infrastructure.Data;
using POS.Infrastructure.Data.Seed;
using POS.Interfaces.Services;
using POS.ViewModels.Common;
using POS.ViewModels.Suppliers;

namespace POS.Services;

public class SupplierService : ISupplierService
{
    private readonly ApplicationDbContext _db;

    public SupplierService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<SupplierListViewModel> GetListAsync(string? query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;
        query = string.IsNullOrWhiteSpace(query) ? null : query.Trim();

        IQueryable<Supplier> baseQuery = _db.Suppliers
            .AsNoTracking()
            .Where(s => s.BranchId == BranchSeed.DefaultBranchId);

        if (query is not null)
        {
            baseQuery = baseQuery.Where(s =>
                s.Name.Contains(query) ||
                (s.Email != null && s.Email.Contains(query)) ||
                (s.Phone != null && s.Phone.Contains(query)));
        }

        var total = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderBy(s => s.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SupplierListItemVm
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Phone = s.Phone,
                PurchasesCount = s.Purchases.Count
            })
            .ToListAsync(cancellationToken);

        return new SupplierListViewModel
        {
            Query = query,
            Results = new PagedResultViewModel<SupplierListItemVm>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total
            }
        };
    }

    public async Task<SupplierEditViewModel?> GetEditModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Suppliers
            .AsNoTracking()
            .Where(s => s.Id == id && s.BranchId == BranchSeed.DefaultBranchId)
            .Select(s => new SupplierEditViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(SupplierUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Supplier
        {
            Id = Guid.NewGuid(),
            BranchId = BranchSeed.DefaultBranchId,
            Name = dto.Name.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
            Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim()
        };

        _db.Suppliers.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, SupplierUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id && s.BranchId == BranchSeed.DefaultBranchId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        entity.Name = dto.Name.Trim();
        entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        entity.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
        entity.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Suppliers
            .Include(s => s.Purchases)
            .FirstOrDefaultAsync(s => s.Id == id && s.BranchId == BranchSeed.DefaultBranchId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        if (entity.Purchases.Count > 0)
        {
            throw new InvalidOperationException("Cannot delete a supplier with purchase history.");
        }

        _db.Suppliers.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

