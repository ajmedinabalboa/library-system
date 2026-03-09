using LibrarySystem.Application.Commands.Books;
using LibrarySystem.Application.Validators;

namespace LibrarySystem.Tests.Validators;

public class CreateBookCommandValidatorTests
{
    private readonly CreateBookCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_For_Valid_Command()
    {
        var command = new CreateBookCommand
        {
            Title = "Clean Architecture",
            Authors = new List<string> { "Robert C. Martin" }
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Pass_With_Publication_Date()
    {
        var command = new CreateBookCommand
        {
            Title = "Design Patterns",
            PublicationDate = new DateTime(1994, 10, 31),
            Authors = new List<string> { "Gang of Four" }
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        var command = new CreateBookCommand
        {
            Title = "",
            Authors = new List<string> { "Author" }
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(CreateBookCommand.Title) &&
            e.ErrorMessage == "Title is required");
    }

    [Fact]
    public void Should_Fail_When_Title_Exceeds_Max_Length()
    {
        var command = new CreateBookCommand
        {
            Title = new string('A', 501),
            Authors = new List<string> { "Author" }
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(CreateBookCommand.Title) &&
            e.ErrorMessage == "Title cannot exceed 500 characters");
    }

    [Fact]
    public void Should_Pass_When_Title_Is_Exactly_Max_Length()
    {
        var command = new CreateBookCommand
        {
            Title = new string('A', 500),
            Authors = new List<string> { "Author" }
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Fail_When_Authors_List_Is_Empty()
    {
        var command = new CreateBookCommand
        {
            Title = "Some Book",
            Authors = new List<string>()
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBookCommand.Authors));
    }

    [Fact]
    public void Should_Pass_With_Multiple_Authors()
    {
        var command = new CreateBookCommand
        {
            Title = "The Book",
            Authors = new List<string> { "Author One", "Author Two" }
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
