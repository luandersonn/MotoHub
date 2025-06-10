using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MotoHub.Domain.Entities;
using MotoHub.Domain.Interfaces;

namespace MotoHub.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Motorcycle> Motorcycles { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Rent> Rents { get; set; } = null!;

    public AppDbContext(DbContextOptions options) : base(options) => SavingChanges += OnSavingChanges;

    private void OnSavingChanges(object? sender, SavingChangesEventArgs e) => UpdateAuditableColunmns();
    private void UpdateAuditableColunmns()
    {
        foreach (EntityEntry<IEntity> entry in ChangeTracker.Entries<IEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    {
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    }

                case EntityState.Modified:
                    {
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                    }

                case EntityState.Deleted:
                    {
                        entry.State = EntityState.Unchanged;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        break;
                    }
            }
        }
    }
}