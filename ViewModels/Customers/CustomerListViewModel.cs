using POS.ViewModels.Common;

namespace POS.ViewModels.Customers;

public class CustomerListViewModel
{
    public string? Query { get; set; }

    public PagedResultViewModel<CustomerListItemVm> Results { get; set; } = new();
}

public class CustomerListItemVm
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int SalesCount { get; set; }
}

