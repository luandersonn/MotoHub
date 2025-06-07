namespace MotoHub.Application.DTOs;

public class ReturnMotorcycleDto
{
    public required string RentIdentifier { get; set; }
    public required DateTime ReturnDate { get; set; }
}
