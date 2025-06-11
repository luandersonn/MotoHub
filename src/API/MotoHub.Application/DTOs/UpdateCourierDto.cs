using MotoHub.Domain.ValueObjects;

namespace MotoHub.Application.DTOs;

public class UpdateCourierDto
{
    public string? Name { get; set; }
    public string? TaxNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public DriverLicenseType? DriverLicenseType { get; set; }
    public string? DriverLicenseImageBase64 { get; set; }
}