namespace MotoHub.Application.DTOs;

public class MotorcycleSearchParameters : PagedSearchParameters
{
    public string? Model { get; set; }
    public string? Plate { get; set; }
    public int? Year { get; }
}