namespace POS.ViewModels.Settings;

public class StoreSettingsDisplayViewModel
{
    public string StoreName { get; set; } = string.Empty;

    public string? StoreAddress { get; set; }

    public string? StorePhone { get; set; }

    public string? StoreEmail { get; set; }

    public decimal TaxRatePercent { get; set; }

    public string? ReceiptFooter { get; set; }

    public string CurrencyCode { get; set; } = "USD";

    public string? CurrencySymbol { get; set; }
}
