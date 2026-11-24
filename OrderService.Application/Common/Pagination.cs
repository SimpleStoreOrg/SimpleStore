namespace OrderService.Application.Common;

public class Pagination<T>
{
    public List<T> Items { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasNext => TotalCount > PageNumber;
    public bool HasPrevious => PageNumber > 1;
}