namespace MotoHub.Domain.Interfaces;

public interface IAuditableEntity<T> : IEntity<T>
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
    DateTimeOffset? DeletedAt { get; set; }

    public T CreatedByUserId { get; set; }
    public T? UpdatedByUserId { get; set; }
    public T? DeletedByUserId { get; set; }
}