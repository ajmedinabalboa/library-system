using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace LibrarySystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Register FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
