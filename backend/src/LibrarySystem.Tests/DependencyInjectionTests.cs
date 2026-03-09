using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using LibrarySystem.Application;
using LibrarySystem.Application.Commands.Auth;
using LibrarySystem.Application.Validators;

namespace LibrarySystem.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_RegistersMediatR()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        // MediatR registers ISender and IMediator into the container
        var hasSender = services.Any(d => d.ServiceType == typeof(ISender));
        Assert.True(hasSender);
    }

    [Fact]
    public void AddApplication_RegistersFluentValidators()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        var provider = services.BuildServiceProvider();

        var validator = provider.GetService<IValidator<LoginCommand>>();
        Assert.NotNull(validator);
        Assert.IsType<LoginCommandValidator>(validator);
    }

    [Fact]
    public void AddApplication_RegistersCreateBookCommandValidator()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        var provider = services.BuildServiceProvider();

        var validator = provider.GetService<IValidator<Application.Commands.Books.CreateBookCommand>>();
        Assert.NotNull(validator);
    }

    [Fact]
    public void AddApplication_ReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();
        var result = services.AddApplication();
        Assert.Same(services, result);
    }
}
