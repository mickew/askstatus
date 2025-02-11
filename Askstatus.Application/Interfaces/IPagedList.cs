namespace Askstatus.Application.Interfaces;
public interface IPagedList<T> where T : class
{
    List<T> Items { get; }
    int Page { get; }
    int PageSize { get; }
    int TotalCount { get; }
    bool HasNextPage { get; }
    bool HasPreviousPage { get; }
}
