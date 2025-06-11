namespace MotoHub.Domain.Interfaces;

public interface IEntity
{
    long Id { get; set; }
    string Identifier { get; set; }
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
    DateTimeOffset? DeletedAt { get; set; }
}