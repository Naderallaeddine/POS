namespace POS.ViewModels.Common;

public class PagedResultViewModel<TItem>
{
    public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPrevious => PageNumber > 1;

    public bool HasNext => PageNumber < TotalPages;
}

