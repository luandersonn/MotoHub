namespace MotoHub.Domain.Entities;

public class Motorcycle : Entity
{
    public string Model { get; set; }
    public int Year { get; set; }
    public string Plate { get; set; }
}