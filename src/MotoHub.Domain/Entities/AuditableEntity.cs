using MotoHub.Domain.Interfaces;

namespace MotoHub.Domain.Entities;

public abstract class AuditableEntity : Entity, IAuditableEntity<long>
{
    public virtual DateTimeOffset CreatedAt { get; set; }
    public virtual DateTimeOffset? UpdatedAt { get; set; }
    public virtual DateTimeOffset? DeletedAt { get; set; }
    public virtual long CreatedByUserId { get; set; }
    public virtual long UpdatedByUserId { get; set; }
    public virtual long DeletedByUserId { get; set; }
}