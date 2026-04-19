using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POS.ViewModels.Purchases;

public class PurchaseCreateViewModel
{
    [Required]
    [Display(Name = "Supplier")]
    public Guid SupplierId { get; set; }

    [Required]
    [Display(Name = "Purchase date")]
    public DateTimeOffset PurchaseDate { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    [StringLength(60)]
    [Display(Name = "Invoice/Reference #")]
    public string ReferenceNumber { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Notes { get; set; }

    public List<PurchaseCreateItemViewModel> Items { get; set; } = new()
    {
        new()
    };

    public IEnumerable<SelectListItem> Suppliers { get; set; } = Array.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> Products { get; set; } = Array.Empty<SelectListItem>();
}

public class PurchaseCreateItemViewModel
{
    [Required]
    [Display(Name = "Product")]
    public Guid ProductId { get; set; }

    [Range(0.001, 999999999)]
    public decimal Quantity { get; set; } = 1;

    [Range(0, 999999999)]
    [Display(Name = "Unit cost")]
    public decimal UnitCost { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Discount")]
    public decimal DiscountAmount { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Tax")]
    public decimal TaxAmount { get; set; }
}

