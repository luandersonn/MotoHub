using MotoHub.Domain.ValueObjects;

namespace MotoHub.Domain.Entities;

public class User : Entity
{
    public string Name { get; set; }
    public string TaxNumber { get; set; }
    public string? PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public DateTime BirthDate { get; set; }
}