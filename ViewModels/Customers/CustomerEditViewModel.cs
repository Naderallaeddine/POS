using System.ComponentModel.DataAnnotations;

namespace POS.ViewModels.Customers;

public class CustomerEditViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(256)]
    public string? Email { get; set; }

    [StringLength(40)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }
}

