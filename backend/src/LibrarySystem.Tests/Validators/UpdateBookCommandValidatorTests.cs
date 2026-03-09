using LibrarySystem.Application.Commands.Books;
using LibrarySystem.Application.Validators;

namespace LibrarySystem.Tests.Validators;

public class UpdateBookCommandValidatorTests
{
    private readonly UpdateBookCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_For_Valid_Command()
    {
        var command = new UpdateBookCommand
        {
            Id = Guid.NewGuid(),
            Title = "Updated Title",
            Authors = new List<string> { "Author Name" }
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Fail_When_Id_Is_Empty_Guid()
    {
        var command = new UpdateBookCommand
        {
            Id = Guid.Empty,
            Title = "Updated Title",
            Authors = new List<string> { "Author" }
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(UpdateBookCommand.Id) &&
            e.ErrorMessage == "Book ID is required");
    }

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        var command = new UpdateBookCommand
        {
            Id = Guid.NewGuid(),
            Title = "",
            Authors = new List<string> { "Author" }
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(UpdateBookCommand.Title) &&
            e.ErrorMessage == "Title is required");
    }

    [Fact]
    public void Should_Fail_When_Title_Exceeds_Max_Length()
    {
        var command = new UpdateBookCommand
        {
            Id = Guid.NewGuid(),
            Title = new string('X', 501),
            Authors = new List<string> { "Author" }
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(UpdateBookCommand.Title) &&
            e.ErrorMessage == "Title cannot exceed 500 characters");
    }

    [Fact]
    public void Should_Fail_When_Authors_Is_Empty()
    {
        var command = new UpdateBookCommand
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            Authors = new List<string>()
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateBookCommand.Authors));
    }

    [Fact]
    public void Should_Report_Multiple_Errors_When_Multiple_Fields_Invalid()
    {
        var command = new UpdateBookCommand
        {
            Id = Guid.Empty,
            Title = "",
            Authors = new List<string>()
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateBookCommand.Id));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateBookCommand.Title));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateBookCommand.Authors));
    }
}
