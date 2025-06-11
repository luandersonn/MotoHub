using MotoHub.Domain.ValueObjects;

namespace MotoHub.Application.DTOs;

public class RegisterCourierDto
{
    public required string Identifier { get; set; }
    public required string Name { get; set; }
    public required string TaxNumber { get; set; }
    public required DateTime BirthDate { get; set; }
    public required string DriverLicenseNumber { get; set; }
    public required DriverLicenseType DriverLicenseType { get; set; }
    public required string DriverLicenseImageBase64 { get; set; }
}
