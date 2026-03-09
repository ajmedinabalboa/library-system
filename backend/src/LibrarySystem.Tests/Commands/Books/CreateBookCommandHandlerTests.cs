using LibrarySystem.Application.Commands.Books;
namespace LibrarySystem.Tests.Commands.Books;

public class CreateBookCommandHandlerTests
{
    /// <summary>
    /// Creates a mock author repository wired to the given context so that
    /// AddAsync actually persists the author into the in-memory store.
    /// </summary>
    private static Mock<IAuthorRepository> CreateAuthorRepoMock(TestDbContext context)
    {
        var mock = new Mock<IAuthorRepository>();
        mock.Setup(r => r.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Author?)null);
        mock.Setup(r => r.AddAsync(It.IsAny<Author>()))
            .Callback<Author>(a => context.Authors.Add(a))
            .ReturnsAsync((Author a) => a);
        return mock;
    }

    [Fact]
    public async Task Handle_NewAuthor_CreatesBookAndAuthor()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var authorMock = CreateAuthorRepoMock(context);
        var handler = new CreateBookCommandHandler(context, authorMock.Object);

        var command = new CreateBookCommand
        {
            Title = "Clean Architecture",
            Authors = new List<string> { "Robert C. Martin" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Clean Architecture", result.Title);
        Assert.Single(result.Authors);
        Assert.Equal("Robert C. Martin", result.Authors[0].Name);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task Handle_ExistingAuthor_ReusesAuthorWithoutCreatingNew()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var existing = new Author { Name = "Martin Fowler" };
        context.Authors.Add(existing);
        await context.SaveChangesAsync();

        var authorMock = new Mock<IAuthorRepository>();
        authorMock.Setup(r => r.GetByNameAsync("Martin Fowler")).ReturnsAsync(existing);

        var handler = new CreateBookCommandHandler(context, authorMock.Object);
        var command = new CreateBookCommand
        {
            Title = "Refactoring",
            Authors = new List<string> { "Martin Fowler" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Single(result.Authors);
        Assert.Equal(existing.Id, result.Authors[0].Id);
        authorMock.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DuplicateAuthorNamesInList_CreatesOnlyOneAuthor()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var authorMock = CreateAuthorRepoMock(context);
        var handler = new CreateBookCommandHandler(context, authorMock.Object);

        var command = new CreateBookCommand
        {
            Title = "The Book",
            Authors = new List<string> { "Same Author", "Same Author" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert – handler uses Distinct(), so AddAsync should be called exactly once
        authorMock.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Once);
        Assert.Single(result.Authors);
    }

    [Fact]
    public async Task Handle_WithPublicationDate_SetsDateInUtc()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var authorMock = CreateAuthorRepoMock(context);
        var handler = new CreateBookCommandHandler(context, authorMock.Object);

        var command = new CreateBookCommand
        {
            Title = "Dated Book",
            PublicationDate = new DateTime(2020, 6, 15),
            Authors = new List<string> { "Author" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result.PublicationDate);
        Assert.Equal(DateTimeKind.Utc, result.PublicationDate!.Value.Kind);
        Assert.Equal(2020, result.PublicationDate.Value.Year);
        Assert.Equal(6, result.PublicationDate.Value.Month);
    }

    [Fact]
    public async Task Handle_MultipleDistinctAuthors_LinksAllAuthors()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var authorMock = CreateAuthorRepoMock(context);
        var handler = new CreateBookCommandHandler(context, authorMock.Object);

        var command = new CreateBookCommand
        {
            Title = "Multi-Author Book",
            Authors = new List<string> { "Author A", "Author B", "Author C" }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Authors.Count);
        Assert.Contains(result.Authors, a => a.Name == "Author A");
        Assert.Contains(result.Authors, a => a.Name == "Author B");
        Assert.Contains(result.Authors, a => a.Name == "Author C");
    }

    [Fact]
    public async Task Handle_ValidCommand_BookIsSavedToDatabase()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var authorMock = CreateAuthorRepoMock(context);
        var handler = new CreateBookCommandHandler(context, authorMock.Object);

        var command = new CreateBookCommand { Title = "Persisted Book", Authors = new List<string> { "Author" } };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var saved = context.Books.Find(result.Id);
        Assert.NotNull(saved);
        Assert.Equal("Persisted Book", saved.Title);
    }
}
