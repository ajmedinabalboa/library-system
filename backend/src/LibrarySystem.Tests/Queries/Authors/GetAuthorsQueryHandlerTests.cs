using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Queries.Authors;

namespace LibrarySystem.Tests.Queries.Authors;

public class GetAuthorsQueryHandlerTests
{
    private readonly Mock<IAuthorRepository> _mockRepo = new();

    [Fact]
    public async Task Handle_NoSearch_ReturnsAllAuthorsSortedByName()
    {
        // Arrange
        var authors = new List<Author>
        {
            new() { Id = Guid.NewGuid(), Name = "Zebra Author" },
            new() { Id = Guid.NewGuid(), Name = "Apple Author" }
        };

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(authors);

        var query = new GetAuthorsQuery { Search = null };

        // Act
        var result = await new GetAuthorsQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Apple Author", result[0].Name);
        Assert.Equal("Zebra Author", result[1].Name);
    }

    [Fact]
    public async Task Handle_NoSearch_ReturnsAtMost20Authors()
    {
        // Arrange
        var authors = Enumerable.Range(1, 25)
            .Select(i => new Author { Id = Guid.NewGuid(), Name = $"Author {i:00}" })
            .ToList();

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(authors);

        var query = new GetAuthorsQuery { Search = null };

        // Act
        var result = await new GetAuthorsQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(20, result.Count);
    }

    [Fact]
    public async Task Handle_WithSearch_DelegatesToSearchByName()
    {
        // Arrange
        var found = new List<Author> { new() { Id = Guid.NewGuid(), Name = "Martin Fowler" } };
        _mockRepo.Setup(r => r.SearchByNameAsync("mart")).ReturnsAsync(found);

        var query = new GetAuthorsQuery { Search = "mart" };

        // Act
        var result = await new GetAuthorsQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Martin Fowler", result[0].Name);
        _mockRepo.Verify(r => r.SearchByNameAsync("mart"), Times.Once);
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithWhitespaceOnlySearch_ReturnsAll()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Author>());

        var query = new GetAuthorsQuery { Search = "   " };

        // Act
        await new GetAuthorsQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        _mockRepo.Verify(r => r.SearchByNameAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MapsAuthorToDto()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Author> { new() { Id = authorId, Name = "Test Author" } });

        var query = new GetAuthorsQuery();

        // Act
        var result = await new GetAuthorsQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(authorId, result[0].Id);
        Assert.Equal("Test Author", result[0].Name);
        Assert.IsType<AuthorDto>(result[0]);
    }
}
