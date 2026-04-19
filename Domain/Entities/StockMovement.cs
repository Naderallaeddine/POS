using POS.Domain.Enums;

namespace POS.Domain.Entities;

public class StockMovement : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }

    public Guid ProductId { get; set; }

    public DateTimeOffset MovementDate { get; set; } = DateTimeOffset.UtcNow;

    public StockMovementType Type { get; set; }

    // Signed quantity. Positive adds stock, negative reduces stock.
    public decimal Quantity { get; set; }

    public string? Reference { get; set; }

    public Guid? SaleId { get; set; }

    public Guid? PurchaseId { get; set; }

    public string? Notes { get; set; }

    public Branch Branch { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public Sale? Sale { get; set; }

    public Purchase? Purchase { get; set; }
}

