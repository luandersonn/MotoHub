using Microsoft.EntityFrameworkCore;
using MotoHub.Domain.Entities;

namespace MotoHub.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Motorcycle> Motorcycles { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
}