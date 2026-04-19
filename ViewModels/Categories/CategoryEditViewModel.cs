using System.ComponentModel.DataAnnotations;

namespace POS.ViewModels.Categories;

public class CategoryEditViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}

