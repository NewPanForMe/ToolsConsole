namespace ToolsConsole.Application.Dtos;

/// <summary>分页请求。</summary>
public sealed class PagedRequest
{
    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}

/// <summary>分页结果。</summary>
public sealed class PagedResult<T>
{
    public List<T> List { get; set; } = new();

    public long Total { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }
}
