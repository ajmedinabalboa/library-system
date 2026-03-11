using FluentValidation;
using FluentValidation.Results;
using MediatR;
using LibrarySystem.Application.Behaviors;

namespace LibrarySystem.Tests.Behaviors;

// Public so Castle.DynamicProxy can create IValidator<T> proxies (Moq requirement)
public sealed record ValidationBehaviorTestRequest : IRequest<string>;

/// <summary>
/// Tests for <see cref="ValidationBehavior{TRequest,TResponse}"/>.
///
/// Verifies that:
/// - When no validators exist the next handler is called unimpeded.
/// - When all validators pass the next handler is called.
/// - When any validator fails a <see cref="ValidationException"/> is thrown
///   before the handler is reached.
/// </summary>
public class ValidationBehaviorTests
{
    private const string HandlerResponse = "handler_called";

    private static RequestHandlerDelegate<string> NextReturnsOk() =>
        () => Task.FromResult(HandlerResponse);

    // ── No validators ───────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<ValidationBehaviorTestRequest, string>(
            Enumerable.Empty<IValidator<ValidationBehaviorTestRequest>>());

        // Act
        var result = await behavior.Handle(
            new ValidationBehaviorTestRequest(), NextReturnsOk(), CancellationToken.None);

        // Assert
        Assert.Equal(HandlerResponse, result);
    }

    // ── Passing validators ──────────────────────────────────────────────────

    [Fact]
    public async Task Handle_AllValidatorsPass_CallsNext()
    {
        // Arrange
        var passing = new Mock<IValidator<ValidationBehaviorTestRequest>>();
        passing
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<ValidationBehaviorTestRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behavior = new ValidationBehavior<ValidationBehaviorTestRequest, string>(
            new[] { passing.Object });

        // Act
        var result = await behavior.Handle(
            new ValidationBehaviorTestRequest(), NextReturnsOk(), CancellationToken.None);

        // Assert
        Assert.Equal(HandlerResponse, result);
    }

    // ── Failing validators ──────────────────────────────────────────────────

    [Fact]
    public async Task Handle_AnyValidatorFails_ThrowsValidationException()
    {
        // Arrange
        var failure = new ValidationFailure("Name", "Name is required");
        var failing = new Mock<IValidator<ValidationBehaviorTestRequest>>();
        failing
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<ValidationBehaviorTestRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { failure }));

        var behavior = new ValidationBehavior<ValidationBehaviorTestRequest, string>(
            new[] { failing.Object });

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(
                new ValidationBehaviorTestRequest(), NextReturnsOk(), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidatorFails_NextIsNeverCalled()
    {
        // Arrange
        var failure = new ValidationFailure("Field", "Error");
        var failing = new Mock<IValidator<ValidationBehaviorTestRequest>>();
        failing
            .Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<ValidationBehaviorTestRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { failure }));

        var nextCalled = false;
        RequestHandlerDelegate<string> nextSpy = () => { nextCalled = true; return Task.FromResult(HandlerResponse); };

        var behavior = new ValidationBehavior<ValidationBehaviorTestRequest, string>(
            new[] { failing.Object });

        // Act
        await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(new ValidationBehaviorTestRequest(), nextSpy, CancellationToken.None));

        // Assert
        Assert.False(nextCalled);
    }

    [Fact]
    public async Task Handle_MultipleValidators_AllAreExecuted()
    {
        // Arrange
        var pass = new Mock<IValidator<ValidationBehaviorTestRequest>>();
        pass.Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<ValidationBehaviorTestRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var fail = new Mock<IValidator<ValidationBehaviorTestRequest>>();
        fail.Setup(v => v.ValidateAsync(
                It.IsAny<ValidationContext<ValidationBehaviorTestRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("F", "E") }));

        var behavior = new ValidationBehavior<ValidationBehaviorTestRequest, string>(
            new[] { pass.Object, fail.Object });

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(new ValidationBehaviorTestRequest(), NextReturnsOk(), CancellationToken.None));

        pass.Verify(v => v.ValidateAsync(
            It.IsAny<ValidationContext<ValidationBehaviorTestRequest>>(),
            It.IsAny<CancellationToken>()), Times.Once);
        fail.Verify(v => v.ValidateAsync(
            It.IsAny<ValidationContext<ValidationBehaviorTestRequest>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
