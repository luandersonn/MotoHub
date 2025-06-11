using MotoHub.Domain.Interfaces;

namespace MotoHub.Domain.Entities;

public abstract class Entity : IEntity
{
    public virtual long Id { get; set; }
    public virtual string Identifier { get; set; }
    public virtual DateTimeOffset CreatedAt { get; set; }
    public virtual DateTimeOffset? UpdatedAt { get; set; }
    public virtual DateTimeOffset? DeletedAt { get; set; }
}