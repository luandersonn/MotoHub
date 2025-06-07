namespace MotoHub.Domain.Entities;

public class Rent : Entity
{
    public string TenantIdentifier { get; set; }
    public string MotorcycleIdentifier { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime EstimatedEndDate { get; set; }
    public decimal PricePerDay { get; set; }
    public RentStatus Status { get; set; }
}