using Microsoft.AspNetCore.Mvc.Rendering;
using POS.Domain.Enums;

namespace POS.ViewModels.Sales;

public class PosViewModel
{
    public IReadOnlyList<SelectListItem> PaymentMethods { get; set; } =
        Enum.GetValues<PaymentMethod>()
            .Select(pm => new SelectListItem(pm.ToString(), ((int)pm).ToString()))
            .ToList();
}

public class ProductSearchResultVm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Price { get; set; }
    public decimal Stock { get; set; }
}

