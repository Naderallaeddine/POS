using System.ComponentModel.DataAnnotations;

namespace POS.DTOs.Categories;

public class CategoryUpsertDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}

