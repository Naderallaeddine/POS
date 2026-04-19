namespace POS.Domain.Entities;

public class Expense : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }

    public DateTimeOffset ExpenseDate { get; set; } = DateTimeOffset.UtcNow;

    public string Title { get; set; } = string.Empty;

    public string? Category { get; set; }

    public decimal Amount { get; set; }

    public string? Notes { get; set; }

    public Branch Branch { get; set; } = null!;
}

