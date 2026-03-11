using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using LibrarySystem.Application.Behaviors;

namespace LibrarySystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register MediatR handlers (CQRS – Command/Query handlers)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        // Register the validation pipeline behavior (Decorator pattern).
        // Every MediatR request passes through this behavior, which runs any
        // registered IValidator<TRequest> before the handler is invoked.
        // This satisfies SOLID – SRP (handlers stay free of validation code).
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Register AutoMapper with the profiles defined in this assembly.
        // Centralising mapping logic here eliminates duplicate Select() projections
        // across command handlers (DRY principle).
        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        return services;
    }
}
