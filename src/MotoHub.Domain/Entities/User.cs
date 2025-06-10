using MotoHub.Domain.ValueObjects;

namespace MotoHub.Domain.Entities;

public class User : Entity
{
    public string Name { get; set; }
    public string TaxNumber { get; set; }
    public string DriverLicenseNumber { get; set; }
    public DriverLicenseType? DriverLicenseType { get; set; }
    public UserRole Role { get; set; }
    public DateTime BirthDate { get; set; }
}