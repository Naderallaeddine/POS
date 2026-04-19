using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POS.ViewModels.Products;

public class ProductEditViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(80)]
    [Display(Name = "Barcode")]
    public string? Barcode { get; set; }

    [Required]
    [Display(Name = "Category")]
    public Guid CategoryId { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Cost price")]
    public decimal CostPrice { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Sale price")]
    public decimal SalePrice { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Stock quantity")]
    public decimal StockQuantity { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public IEnumerable<SelectListItem> Categories { get; set; } = Array.Empty<SelectListItem>();
}

