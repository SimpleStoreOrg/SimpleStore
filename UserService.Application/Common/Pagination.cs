namespace UserService.Application.Common;

public class Pagination<T>
{
    public List<T> Items { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}