using System.ComponentModel.DataAnnotations;
using POS.Domain.Enums;

namespace POS.DTOs.Sales;

public class SaleCreateDto
{
    public Guid? CustomerId { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [Range(0, 999999999)]
    public decimal DiscountAmount { get; set; }

    [Range(0, 999999999)]
    public decimal TaxAmount { get; set; }

    [Range(0, 999999999)]
    public decimal PaidAmount { get; set; }

    [MinLength(1, ErrorMessage = "Add at least one item.")]
    public List<SaleCreateItemDto> Items { get; set; } = new();
}

public class SaleCreateItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(0.001, 999999999)]
    public decimal Quantity { get; set; }
}

