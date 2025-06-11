namespace MotoHub.Application.DTOs;

public abstract class PagedSearchParameters
{
    public string? Identifier { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}
