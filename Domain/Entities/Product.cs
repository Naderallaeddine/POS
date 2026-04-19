namespace POS.Domain.Entities;

public class Product : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }

    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Sku { get; set; }

    public string? Barcode { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal CurrentStock { get; set; }

    public decimal ReorderLevel { get; set; }

    public bool IsActive { get; set; } = true;

    public Branch Branch { get; set; } = null!;

    public Category Category { get; set; } = null!;

    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}

