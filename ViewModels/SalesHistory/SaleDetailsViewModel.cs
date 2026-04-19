namespace POS.ViewModels.SalesHistory;

public class SaleDetailsViewModel
{
    public Guid Id { get; set; }
    public DateTimeOffset SaleDate { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public string Customer { get; set; } = "Walk-in";
    public string PaymentMethod { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal ChangeAmount { get; set; }

    public IReadOnlyList<SaleDetailsItemVm> Items { get; set; } = Array.Empty<SaleDetailsItemVm>();
}

public class SaleDetailsItemVm
{
    public string ProductName { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

