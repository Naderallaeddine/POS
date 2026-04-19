using System.ComponentModel.DataAnnotations;

namespace POS.ViewModels.Settings;

public class StoreSettingsEditViewModel
{
    [Required]
    [StringLength(200)]
    [Display(Name = "Store name")]
    public string StoreName { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Address")]
    public string? StoreAddress { get; set; }

    [StringLength(40)]
    [Display(Name = "Phone")]
    public string? StorePhone { get; set; }

    [EmailAddress]
    [StringLength(256)]
    [Display(Name = "Email")]
    public string? StoreEmail { get; set; }

    [Range(0, 100)]
    [Display(Name = "Tax rate (%)")]
    public decimal TaxRatePercent { get; set; }

    [StringLength(1000)]
    [Display(Name = "Receipt footer")]
    public string? ReceiptFooter { get; set; }

    [Required]
    [StringLength(8)]
    [Display(Name = "Currency code")]
    public string CurrencyCode { get; set; } = "USD";

    [StringLength(8)]
    [Display(Name = "Currency symbol")]
    public string? CurrencySymbol { get; set; }
}
