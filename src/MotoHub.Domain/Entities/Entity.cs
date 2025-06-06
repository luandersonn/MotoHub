using MotoHub.Domain.Interfaces;

namespace MotoHub.Domain.Entities;

public abstract class Entity : IEntity<long>
{
    public virtual long? Id { get; set; }
    public virtual object? GetId() => Id;

    public void SetId(object? id)
    {
        Id = id is IConvertible convertibleId
            ? convertibleId.ToInt64(null)
            : throw new ArgumentException("Id must be convertible to long.", nameof(id));
    }

    public void SetId(long id) => Id = id;

    long IEntity<long>.GetId() => Id ?? throw new InvalidOperationException("Id is not set.");
}