using POS.Domain.Enums;

namespace POS.Domain.Entities;

public class Sale : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }

    public Guid? CustomerId { get; set; }

    public DateTimeOffset SaleDate { get; set; } = DateTimeOffset.UtcNow;

    public string ReceiptNumber { get; set; } = string.Empty;

    public PaymentMethod PaymentMethod { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal Total { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal ChangeAmount { get; set; }

    public string? Notes { get; set; }

    public Branch Branch { get; set; } = null!;

    public Customer? Customer { get; set; }

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}

