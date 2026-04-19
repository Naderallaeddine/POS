namespace POS.Domain.Entities;

public class StoreSettings : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid BranchId { get; set; }

    public string StoreName { get; set; } = string.Empty;

    public string? StoreAddress { get; set; }

    public string? StorePhone { get; set; }

    public string? StoreEmail { get; set; }

    /// <summary>
    /// Tax rate as a percentage (e.g. 10.5 means 10.5%).
    /// </summary>
    public decimal TaxRatePercent { get; set; }

    public string? ReceiptFooter { get; set; }

    /// <summary>
    /// ISO 4217 currency code (e.g. USD, EUR).
    /// </summary>
    public string CurrencyCode { get; set; } = "USD";

    /// <summary>
    /// Optional display symbol (e.g. $). Used when currency formatting is not available.
    /// </summary>
    public string? CurrencySymbol { get; set; }

    public Branch Branch { get; set; } = null!;
}
