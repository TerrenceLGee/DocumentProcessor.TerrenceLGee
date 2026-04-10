namespace DocumentProcessor.Avalonia.TerrenceLGee.Common.Parameters;

public class PaginationParams
{
    const int MaxPageSize = 50;
    public int Page { get; set; } = 1;
    private int _pageSize = 10;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
