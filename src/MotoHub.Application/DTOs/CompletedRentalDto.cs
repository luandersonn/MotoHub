using MotoHub.Domain.Entities;

namespace MotoHub.Application.DTOs;

public class CompletedRentalDto
{
    public required string Identifier { get; set; }
    public required string MotorcycleIdentifier { get; set; }
    public required string TenantIdentifier { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalCost { get; set; }
    public RentStatus Status { get; set; }
}