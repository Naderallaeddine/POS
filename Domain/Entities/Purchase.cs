namespace POS.Domain.Entities;

public class Purchase : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }

    public Guid SupplierId { get; set; }

    public DateTimeOffset PurchaseDate { get; set; } = DateTimeOffset.UtcNow;

    public string ReferenceNumber { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal Total { get; set; }

    public string? Notes { get; set; }

    public Branch Branch { get; set; } = null!;

    public Supplier Supplier { get; set; } = null!;

    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();

    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}

