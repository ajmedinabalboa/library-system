using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Queries.Books;

namespace LibrarySystem.Tests.Queries.Books;

public class GetBookByIdQueryHandlerTests
{
    private readonly Mock<IBookRepository> _mockRepo = new();

    [Fact]
    public async Task Handle_ExistingBook_ReturnsBookDto()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var expected = new BookDto
        {
            Id = bookId,
            Title = "Found Book",
            Authors = new List<AuthorDto> { new() { Id = Guid.NewGuid(), Name = "Some Author" } }
        };

        _mockRepo.Setup(r => r.GetBookByIdAsync(bookId)).ReturnsAsync(expected);

        var query = new GetBookByIdQuery { Id = bookId };

        // Act
        var result = await new GetBookByIdQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bookId, result.Id);
        Assert.Equal("Found Book", result.Title);
        Assert.Single(result.Authors);
    }

    [Fact]
    public async Task Handle_NonExistingBook_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetBookByIdAsync(It.IsAny<Guid>())).ReturnsAsync((BookDto?)null);

        var query = new GetBookByIdQuery { Id = Guid.NewGuid() };

        // Act
        var result = await new GetBookByIdQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithExactId()
    {
        // Arrange
        var specificId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetBookByIdAsync(specificId)).ReturnsAsync((BookDto?)null);

        var query = new GetBookByIdQuery { Id = specificId };

        // Act
        await new GetBookByIdQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.GetBookByIdAsync(specificId), Times.Once);
    }

    [Fact]
    public async Task Handle_BookWithMultipleAuthors_ReturnsAllAuthors()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new BookDto
        {
            Id = bookId,
            Title = "Multi-Author",
            Authors = new List<AuthorDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Author 1" },
                new() { Id = Guid.NewGuid(), Name = "Author 2" },
                new() { Id = Guid.NewGuid(), Name = "Author 3" }
            }
        };

        _mockRepo.Setup(r => r.GetBookByIdAsync(bookId)).ReturnsAsync(book);

        var query = new GetBookByIdQuery { Id = bookId };

        // Act
        var result = await new GetBookByIdQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Authors.Count);
        Assert.Equal(3, result.AuthorCount);
    }
}
