namespace POS.Domain.Entities;

public class PurchaseItem : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }

    public Guid PurchaseId { get; set; }

    public Guid ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal LineTotal { get; set; }

    public Branch Branch { get; set; } = null!;

    public Purchase Purchase { get; set; } = null!;

    public Product Product { get; set; } = null!;
}

