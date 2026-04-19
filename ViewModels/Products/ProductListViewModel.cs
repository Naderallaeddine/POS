namespace POS.ViewModels.Products;

public class ProductListViewModel
{
    public IReadOnlyList<ProductListItemVm> Products { get; set; } = Array.Empty<ProductListItemVm>();
}

public class ProductListItemVm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal StockQuantity { get; set; }
    public bool IsActive { get; set; }
}

