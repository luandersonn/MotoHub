using MotoHub.Application.Interfaces.UseCases;
using MotoHub.Application.UseCases;

namespace MotoHub.API.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddUseCases();
        
        return services;
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<ISearchMotorcyclesUseCase, SearchMotorcyclesUseCase>();
        services.AddScoped<IRegisterMotorcycleUseCase, RegisterMotorcycleUseCase>();
        services.AddScoped<IGetMotorcycleByIdentifierUseCase, GetMotorcycleByIdentifierUseCase>();
        services.AddScoped<IDeleteMotorcycleUseCase, DeleteMotorcycleUseCase>();

        return services;
    }
}