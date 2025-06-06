namespace MotoHub.Domain.Interfaces;


public interface IEntity
{
    void SetId(object? id);
    object? GetId();
}

public interface IEntity<T> : IEntity
{
    void SetId(T? id);
    new T? GetId();
}