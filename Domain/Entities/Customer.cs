namespace POS.Domain.Entities;

public class Customer : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public Branch Branch { get; set; } = null!;

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}

