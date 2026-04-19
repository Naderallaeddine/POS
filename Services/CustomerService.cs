using Microsoft.EntityFrameworkCore;
using POS.DTOs.Customers;
using POS.Domain.Entities;
using POS.Infrastructure.Data;
using POS.Infrastructure.Data.Seed;
using POS.Interfaces.Services;
using POS.ViewModels.Customers;
using POS.ViewModels.Common;

namespace POS.Services;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _db;

    public CustomerService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CustomerListViewModel> GetListAsync(string? query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;
        query = string.IsNullOrWhiteSpace(query) ? null : query.Trim();

        IQueryable<Customer> baseQuery = _db.Customers
            .AsNoTracking()
            .Where(c => c.BranchId == BranchSeed.DefaultBranchId);

        if (query is not null)
        {
            baseQuery = baseQuery.Where(c =>
                c.FullName.Contains(query) ||
                (c.Email != null && c.Email.Contains(query)) ||
                (c.Phone != null && c.Phone.Contains(query)));
        }

        var total = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderBy(c => c.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CustomerListItemVm
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                SalesCount = c.Sales.Count
            })
            .ToListAsync(cancellationToken);

        return new CustomerListViewModel
        {
            Query = query,
            Results = new PagedResultViewModel<CustomerListItemVm>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total
            }
        };
    }

    public async Task<CustomerEditViewModel?> GetEditModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Customers
            .AsNoTracking()
            .Where(c => c.Id == id && c.BranchId == BranchSeed.DefaultBranchId)
            .Select(c => new CustomerEditViewModel
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(CustomerUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Customer
        {
            Id = Guid.NewGuid(),
            BranchId = BranchSeed.DefaultBranchId,
            FullName = dto.FullName.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
            Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim()
        };

        _db.Customers.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, CustomerUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.BranchId == BranchSeed.DefaultBranchId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        entity.FullName = dto.FullName.Trim();
        entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        entity.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
        entity.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Customers
            .Include(c => c.Sales)
            .FirstOrDefaultAsync(c => c.Id == id && c.BranchId == BranchSeed.DefaultBranchId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        if (entity.Sales.Count > 0)
        {
            throw new InvalidOperationException("Cannot delete a customer with sales history.");
        }

        _db.Customers.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

