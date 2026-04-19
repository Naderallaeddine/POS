using System.ComponentModel.DataAnnotations;

namespace POS.DTOs.Products;

public class ProductUpsertDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Barcode { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Range(0, 999999999)]
    public decimal CostPrice { get; set; }

    [Range(0, 999999999)]
    public decimal SalePrice { get; set; }

    [Range(0, 999999999)]
    public decimal StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;
}

