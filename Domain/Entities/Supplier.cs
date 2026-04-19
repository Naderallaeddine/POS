namespace POS.Domain.Entities;

public class Supplier : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public Branch Branch { get; set; } = null!;

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}

