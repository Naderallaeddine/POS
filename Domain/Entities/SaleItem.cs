namespace POS.Domain.Entities;

public class SaleItem : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }

    public Guid SaleId { get; set; }

    public Guid ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal LineTotal { get; set; }

    public Branch Branch { get; set; } = null!;

    public Sale Sale { get; set; } = null!;

    public Product Product { get; set; } = null!;
}

