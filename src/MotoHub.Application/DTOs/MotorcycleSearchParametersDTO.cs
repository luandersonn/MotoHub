namespace MotoHub.Application.DTOs;

public class MotorcycleSearchParametersDTO
{
    public string? Identifier { get; set; }
    public string? Model { get; set; }
    public string? Plate { get; set; }
    public int? Year { get; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}