using MotoHub.Domain.Interfaces;

namespace MotoHub.Domain.Entities;

public abstract class Entity : IEntity
{    
    public virtual string Id { get; set; }
    public virtual DateTimeOffset CreatedAt { get; set; }
    public virtual DateTimeOffset? UpdatedAt { get; set; }
    public virtual DateTimeOffset? DeletedAt { get; set; }
}