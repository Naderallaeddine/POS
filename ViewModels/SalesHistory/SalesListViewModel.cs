using POS.ViewModels.Common;

namespace POS.ViewModels.SalesHistory;

public class SalesListViewModel
{
    public string? Query { get; set; }

    public DateTimeOffset? DateFrom { get; set; }

    public DateTimeOffset? DateTo { get; set; }

    public PagedResultViewModel<SalesListItemVm> Results { get; set; } = new();
}

public class SalesListItemVm
{
    public Guid Id { get; set; }
    public DateTimeOffset SaleDate { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public string Customer { get; set; } = "Walk-in";
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

