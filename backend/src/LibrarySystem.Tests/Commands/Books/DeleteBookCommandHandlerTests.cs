using LibrarySystem.Application.Commands.Books;
namespace LibrarySystem.Tests.Commands.Books;

public class DeleteBookCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingBook_ReturnsTrueAndRemovesBook()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var book = new Book { Title = "To Delete" };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var command = new DeleteBookCommand { Id = book.Id };

        // Act
        var result = await new DeleteBookCommandHandler(context).Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Null(context.Books.Find(book.Id));
    }

    [Fact]
    public async Task Handle_NonExistingBook_ReturnsFalse()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var command = new DeleteBookCommand { Id = Guid.NewGuid() };

        // Act
        var result = await new DeleteBookCommandHandler(context).Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ExistingBook_RemovesBookAuthorRelationships()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var author = new Author { Name = "Some Author" };
        context.Authors.Add(author);

        var book = new Book { Title = "Book With Authors" };
        context.Books.Add(book);

        context.BookAuthors.Add(new BookAuthor { BookId = book.Id, AuthorId = author.Id });
        await context.SaveChangesAsync();

        var command = new DeleteBookCommand { Id = book.Id };

        // Act
        var result = await new DeleteBookCommandHandler(context).Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        var remaining = context.BookAuthors.Where(ba => ba.BookId == book.Id).ToList();
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task Handle_ExistingBook_DoesNotDeleteUnrelatedBooks()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var book1 = new Book { Title = "Book 1" };
        var book2 = new Book { Title = "Book 2 - Should Remain" };
        context.Books.AddRange(book1, book2);
        await context.SaveChangesAsync();

        var command = new DeleteBookCommand { Id = book1.Id };

        // Act
        await new DeleteBookCommandHandler(context).Handle(command, CancellationToken.None);

        // Assert
        var remaining = context.Books.Find(book2.Id);
        Assert.NotNull(remaining);
        Assert.Equal("Book 2 - Should Remain", remaining.Title);
    }
}
