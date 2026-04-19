using System.ComponentModel.DataAnnotations;

namespace POS.DTOs.Customers;

public class CustomerUpsertDto
{
    [Required]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(256)]
    public string? Email { get; set; }

    [StringLength(40)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }
}

