namespace POS.Domain.Entities;

public class Branch : AuditableEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Category> Categories { get; set; } = new List<Category>();

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}

