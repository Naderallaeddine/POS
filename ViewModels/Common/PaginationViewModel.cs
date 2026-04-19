namespace POS.ViewModels.Common;

public class PaginationViewModel
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }

    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}

