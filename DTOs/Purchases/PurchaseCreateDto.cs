using System.ComponentModel.DataAnnotations;

namespace POS.DTOs.Purchases;

public class PurchaseCreateDto
{
    [Required]
    public Guid SupplierId { get; set; }

    [Required]
    public DateTimeOffset PurchaseDate { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    [StringLength(60)]
    public string ReferenceNumber { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Notes { get; set; }

    [MinLength(1, ErrorMessage = "Add at least one item.")]
    public List<PurchaseCreateItemDto> Items { get; set; } = new();
}

public class PurchaseCreateItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(0.001, 999999999)]
    public decimal Quantity { get; set; }

    [Range(0, 999999999)]
    public decimal UnitCost { get; set; }

    [Range(0, 999999999)]
    public decimal DiscountAmount { get; set; }

    [Range(0, 999999999)]
    public decimal TaxAmount { get; set; }
}

