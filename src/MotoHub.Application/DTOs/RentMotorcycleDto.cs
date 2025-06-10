namespace MotoHub.Application.DTOs;

public class RentMotorcycleDto
{
    public string? Identifier { get; set; }
    public required string CourierIdentifier { get; set; }
    public required string MotorcycleIdentifier { get; set; }
    public required int Plan { get; set; }
}