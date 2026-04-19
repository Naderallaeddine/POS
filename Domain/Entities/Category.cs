namespace POS.Domain.Entities;

public class Category : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Branch Branch { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

