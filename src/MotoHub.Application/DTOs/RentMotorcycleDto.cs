namespace MotoHub.Application.DTOs;

public class RentMotorcycleDto
{
    public required string TenantIdentifier { get; set; }
    public required string MotorcycleIdentifier { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required DateTime EstimatedEndDate { get; set; }
    public required int Plan { get; set; }
}