using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.UseCases.Couriers;
using MotoHub.Application.Interfaces.UseCases.Motorcycles;
using MotoHub.Application.Interfaces.UseCases.Renting;
using MotoHub.Application.Services;
using MotoHub.Application.UseCases.Couriers;
using MotoHub.Application.UseCases.Motorcycles;
using MotoHub.Application.UseCases.Renting;

namespace MotoHub.API.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddUseCases();

        services.AddSingleton<IRentPricingCalculator, DefaultRentPricingCalculator>();

        return services;
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        // Motorcycle
        services.AddScoped<ISearchMotorcyclesUseCase, SearchMotorcyclesUseCase>();
        services.AddScoped<IRegisterMotorcycleUseCase, RegisterMotorcycleUseCase>();
        services.AddScoped<IUpdateMotorcycleUseCase, UpdateMotorcycleUseCase>();
        services.AddScoped<IGetMotorcycleByIdentifierUseCase, GetMotorcycleByIdentifierUseCase>();
        services.AddScoped<IDeleteMotorcycleUseCase, DeleteMotorcycleUseCase>();

        // Courier
        services.AddScoped<IRegisterCourierUseCase, RegisterCourierUseCase>();
        services.AddScoped<IUpdateCourierUseCase, UpdateCourierUseCase>();

        // Renting
        services.AddScoped<IRentMotorcycleUseCase, RentMotorcycleUseCase>();
        services.AddScoped<IGetRentByIdentifierUseCase, GetRentByIdentifierUseCase>();
        services.AddScoped<IReturnMotorcycleUseCase, ReturnMotorcycleUseCase>();

        return services;
    }
}