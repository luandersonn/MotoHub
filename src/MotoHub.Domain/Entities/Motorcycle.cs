namespace MotoHub.Domain.Entities;

public class Motorcycle : AuditableEntity
{
    public string Identifier { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Plate { get; set; }
}