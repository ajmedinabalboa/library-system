using FluentValidation;
using MediatR;

namespace LibrarySystem.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that automatically validates every incoming
/// command or query before it reaches its handler.
///
/// Design patterns applied:
/// - <b>Decorator / Chain-of-Responsibility</b>: wraps the next handler in the
///   pipeline to inject cross-cutting validation logic.
/// - <b>SOLID – Single Responsibility Principle (SRP)</b>: validation is
///   handled here, keeping individual command handlers free of validation code.
/// - <b>SOLID – Open/Closed Principle (OCP)</b>: new validators are picked up
///   automatically without changing this class.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
