namespace POS.ViewModels.Categories;

public class CategoryListViewModel
{
    public IReadOnlyList<CategoryListItemVm> Categories { get; set; } = Array.Empty<CategoryListItemVm>();
}

public class CategoryListItemVm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProductsCount { get; set; }
}

