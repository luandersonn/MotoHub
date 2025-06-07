namespace MotoHub.Application.DTOs;

public class CourierDto
{
    public required string Identifier { get; set; }
    public required string Name { get; set; }
    public string? TaxNumber { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public string? DriverLicenseType { get; set; }
    public string? DriverLicenseImage { get; set; }
}