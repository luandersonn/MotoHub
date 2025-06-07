using Microsoft.EntityFrameworkCore;
using MotoHub.Application.Interfaces;
using MotoHub.Infrastructure.Persistence;

namespace MotoHub.API.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(static options => options.UseSqlite("Data Source=motohub.db"));

        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();

        return services;
    }

    public static void EnsureDatabaseCreated(this IHost app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
}