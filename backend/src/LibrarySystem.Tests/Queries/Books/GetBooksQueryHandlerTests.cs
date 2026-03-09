using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Queries.Books;
namespace LibrarySystem.Tests.Queries.Books;

public class GetBooksQueryHandlerTests
{
    private readonly Mock<IBookRepository> _mockRepo = new();

    [Fact]
    public async Task Handle_DefaultQuery_ReturnsRepositoryResult()
    {
        // Arrange
        var expected = new PaginatedResult<BookDto>
        {
            Items = new List<BookDto> { new() { Id = Guid.NewGuid(), Title = "Test Book" } },
            Page = 1,
            PageSize = 10,
            TotalCount = 1
        };

        _mockRepo
            .Setup(r => r.GetBooksAsync(1, 10, null, null, null, null, null, null, "asc"))
            .ReturnsAsync(expected);

        var query = new GetBooksQuery();

        // Act
        var result = await new GetBooksQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(expected, result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task Handle_ForwardsAllFiltersToRepository()
    {
        // Arrange
        var query = new GetBooksQuery
        {
            Page = 2,
            PageSize = 5,
            Title = "Clean",
            Authors = new List<string> { "Martin" },
            SortBy = "title",
            SortOrder = "desc"
        };

        _mockRepo
            .Setup(r => r.GetBooksAsync(2, 5, "Clean", It.IsAny<List<string>>(), null, null, null, "title", "desc"))
            .ReturnsAsync(new PaginatedResult<BookDto>());

        // Act
        await new GetBooksQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        _mockRepo.Verify(
            r => r.GetBooksAsync(2, 5, "Clean", It.IsAny<List<string>>(), null, null, null, "title", "desc"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ForwardsDateFiltersToRepository()
    {
        // Arrange
        var after = new DateTime(2020, 1, 1);
        var before = new DateTime(2024, 12, 31);
        var query = new GetBooksQuery
        {
            PublishedAfter = after,
            PublishedBefore = before,
            MinAuthors = 2
        };

        _mockRepo
            .Setup(r => r.GetBooksAsync(1, 10, null, null, after, before, 2, null, "asc"))
            .ReturnsAsync(new PaginatedResult<BookDto>());

        // Act
        await new GetBooksQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        _mockRepo.Verify(
            r => r.GetBooksAsync(1, 10, null, null, after, before, 2, null, "asc"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsPaginatedResultWithZeroTotalPages()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.GetBooksAsync(It.IsAny<int>(), It.IsAny<int>(),
                null, null, null, null, null, null, It.IsAny<string>()))
            .ReturnsAsync(new PaginatedResult<BookDto> { Page = 1, PageSize = 10, TotalCount = 0 });

        var query = new GetBooksQuery();

        // Act
        var result = await new GetBooksQueryHandler(_mockRepo.Object).Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }
}
