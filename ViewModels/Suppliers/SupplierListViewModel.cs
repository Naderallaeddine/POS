using POS.ViewModels.Common;

namespace POS.ViewModels.Suppliers;

public class SupplierListViewModel
{
    public string? Query { get; set; }

    public PagedResultViewModel<SupplierListItemVm> Results { get; set; } = new();
}

public class SupplierListItemVm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int PurchasesCount { get; set; }
}

