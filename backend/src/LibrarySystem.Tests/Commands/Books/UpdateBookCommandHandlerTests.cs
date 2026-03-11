using AutoMapper;
using LibrarySystem.Application.Commands.Books;
using LibrarySystem.Application.Mappings;
using LibrarySystem.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
namespace LibrarySystem.Tests.Commands.Books;

public class UpdateBookCommandHandlerTests
{
    private static IMapper CreateMapper() =>
        new Mapper(new MapperConfiguration(
            cfg => cfg.AddProfile<BookMappingProfile>(),
            NullLoggerFactory.Instance));

    private static async Task<(TestDbContext context, Book book)> SeedBookAsync(
        string title = "Original Title",
        string authorName = "Original Author")
    {
        var context = TestDbContext.Create();

        var author = new Author { Name = authorName };
        context.Authors.Add(author);

        var book = new Book { Title = title };
        context.Books.Add(book);

        context.BookAuthors.Add(new BookAuthor { BookId = book.Id, AuthorId = author.Id });

        await context.SaveChangesAsync();
        return (context, book);
    }

    private static Mock<IAuthorRepository> CreateNewAuthorMock(TestDbContext context)
    {
        var mock = new Mock<IAuthorRepository>();
        mock.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((Author?)null);
        mock.Setup(r => r.AddAsync(It.IsAny<Author>()))
            .Callback<Author>(a => context.Authors.Add(a))
            .ReturnsAsync((Author a) => a);
        return mock;
    }

    [Fact]
    public async Task Handle_ExistingBook_UpdatesTitle()
    {
        // Arrange
        var (context, book) = await SeedBookAsync("Old Title");
        await using var _ = context;
        var authorMock = CreateNewAuthorMock(context);

        var command = new UpdateBookCommand
        {
            Id = book.Id,
            Title = "New Title",
            Authors = new List<string> { "Any Author" }
        };

        // Act
        var result = await new UpdateBookCommandHandler(context, authorMock.Object, CreateMapper()).Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("New Title", result.Title);
    }

    [Fact]
    public async Task Handle_ExistingBook_ReplacesAllAuthors()
    {
        // Arrange
        var (context, book) = await SeedBookAsync("Book", "Old Author");
        await using var _ = context;
        var authorMock = CreateNewAuthorMock(context);

        var command = new UpdateBookCommand
        {
            Id = book.Id,
            Title = "Book",
            Authors = new List<string> { "New Author" }
        };

        // Act
        var result = await new UpdateBookCommandHandler(context, authorMock.Object, CreateMapper()).Handle(command, CancellationToken.None);

        // Assert
        Assert.Single(result.Authors);
        Assert.Equal("New Author", result.Authors[0].Name);
    }

    [Fact]
    public async Task Handle_ExistingBook_SetsUpdatedAt()
    {
        // Arrange
        var (context, book) = await SeedBookAsync();
        await using var _ = context;
        var authorMock = CreateNewAuthorMock(context);
        var beforeUpdate = DateTime.UtcNow;

        var command = new UpdateBookCommand
        {
            Id = book.Id,
            Title = "Updated",
            Authors = new List<string> { "Author" }
        };

        // Act
        var result = await new UpdateBookCommandHandler(context, authorMock.Object, CreateMapper()).Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result.UpdatedAt);
        Assert.True(result.UpdatedAt >= beforeUpdate);
    }

    [Fact]
    public async Task Handle_ExistingBook_UpdatesPublicationDate()
    {
        // Arrange
        var (context, book) = await SeedBookAsync();
        await using var _ = context;
        var authorMock = CreateNewAuthorMock(context);

        var command = new UpdateBookCommand
        {
            Id = book.Id,
            Title = "Title",
            PublicationDate = new DateTime(2023, 3, 1),
            Authors = new List<string> { "Author" }
        };

        // Act
        var result = await new UpdateBookCommandHandler(context, authorMock.Object, CreateMapper()).Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result.PublicationDate);
        Assert.Equal(2023, result.PublicationDate!.Value.Year);
    }

    [Fact]
    public async Task Handle_NonExistingBook_ThrowsBookNotFoundException()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var authorMock = new Mock<IAuthorRepository>();

        var missingId = Guid.NewGuid();
        var command = new UpdateBookCommand
        {
            Id = missingId,
            Title = "Ghost Book",
            Authors = new List<string> { "Author" }
        };

        // Act & Assert – domain exception replaces generic KeyNotFoundException
        var ex = await Assert.ThrowsAsync<BookNotFoundException>(
            () => new UpdateBookCommandHandler(context, authorMock.Object, CreateMapper()).Handle(command, CancellationToken.None));

        Assert.Equal(missingId, ex.BookId);
    }

    [Fact]
    public async Task Handle_ExistingAuthor_ReusesWithoutCreatingNew()
    {
        // Arrange
        var (context, book) = await SeedBookAsync("Book", "Old Author");
        await using var _ = context;

        var existing = new Author { Name = "Existing Author" };
        context.Authors.Add(existing);
        await context.SaveChangesAsync();

        var authorMock = new Mock<IAuthorRepository>();
        authorMock.Setup(r => r.GetByNameAsync("Existing Author")).ReturnsAsync(existing);

        var command = new UpdateBookCommand
        {
            Id = book.Id,
            Title = "Book",
            Authors = new List<string> { "Existing Author" }
        };

        // Act
        await new UpdateBookCommandHandler(context, authorMock.Object, CreateMapper()).Handle(command, CancellationToken.None);

        // Assert
        authorMock.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Never);
    }
}
