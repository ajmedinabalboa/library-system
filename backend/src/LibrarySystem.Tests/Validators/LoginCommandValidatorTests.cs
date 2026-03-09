using LibrarySystem.Application.Commands.Auth;
using LibrarySystem.Application.Validators;

namespace LibrarySystem.Tests.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_For_Valid_Credentials()
    {
        var command = new LoginCommand { Email = "user@example.com", Password = "password123" };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Fail_When_Email_Is_Empty()
    {
        var command = new LoginCommand { Email = "", Password = "password123" };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginCommand.Email));
    }

    [Fact]
    public void Should_Fail_When_Email_Format_Is_Invalid()
    {
        var command = new LoginCommand { Email = "not-an-email", Password = "password123" };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(LoginCommand.Email) &&
            e.ErrorMessage == "Invalid email format");
    }

    [Fact]
    public void Should_Fail_When_Password_Is_Empty()
    {
        var command = new LoginCommand { Email = "user@example.com", Password = "" };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginCommand.Password));
    }

    [Fact]
    public void Should_Fail_When_Password_Is_Too_Short()
    {
        var command = new LoginCommand { Email = "user@example.com", Password = "abc" };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(LoginCommand.Password) &&
            e.ErrorMessage == "Password must be at least 6 characters");
    }
}
