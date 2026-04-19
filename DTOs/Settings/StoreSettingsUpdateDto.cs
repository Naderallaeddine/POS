using System.ComponentModel.DataAnnotations;

namespace POS.DTOs.Settings;

public class StoreSettingsUpdateDto
{
    [Required]
    [StringLength(200)]
    public string StoreName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? StoreAddress { get; set; }

    [StringLength(40)]
    public string? StorePhone { get; set; }

    [EmailAddress]
    [StringLength(256)]
    public string? StoreEmail { get; set; }

    [Range(0, 100)]
    public decimal TaxRatePercent { get; set; }

    [StringLength(1000)]
    public string? ReceiptFooter { get; set; }

    [Required]
    [StringLength(8)]
    public string CurrencyCode { get; set; } = "USD";

    [StringLength(8)]
    public string? CurrencySymbol { get; set; }
}
