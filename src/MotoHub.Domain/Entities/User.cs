using MotoHub.Domain.ValueObjects;

namespace MotoHub.Domain.Entities;

public class User : AuditableEntity
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public UserRole Role { get; set; }
}