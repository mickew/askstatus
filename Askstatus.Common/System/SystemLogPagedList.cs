namespace Askstatus.Common.System;
public sealed class SystemLogPagedList
{
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public List<SystemLogDto> Data { get; set; } = new List<SystemLogDto>();
}
