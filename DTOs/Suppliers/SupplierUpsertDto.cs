using System.ComponentModel.DataAnnotations;

namespace POS.DTOs.Suppliers;

public class SupplierUpsertDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(256)]
    public string? Email { get; set; }

    [StringLength(40)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }
}

