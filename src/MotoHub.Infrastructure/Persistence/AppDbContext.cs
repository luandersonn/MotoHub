using Microsoft.EntityFrameworkCore;
using MotoHub.Domain.Entities;

namespace MotoHub.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected AppDbContext()
    {
    }

    public DbSet<Motorcycle> Motorcycles { get; set; } = null!;
}