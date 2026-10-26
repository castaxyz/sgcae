namespace SGCAE.Application.Common.Models;

/// <summary>
/// Envoltorio de paginación para queries que devuelven listas.
/// </summary>
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;

    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>Crea un resultado paginado vacío.</summary>
    public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 10)
        => new(Array.Empty<T>(), 0, pageNumber, pageSize);
}
