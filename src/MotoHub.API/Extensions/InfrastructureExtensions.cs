using Microsoft.EntityFrameworkCore;
using MotoHub.Application.Interfaces;
using MotoHub.Infrastructure.Persistence;

namespace MotoHub.API.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
    {

        services.AddDbContext<AppDbContext>(options =>            
            options.UseInMemoryDatabase("MotoHubDb"));

            services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();

        return services;
    }
}