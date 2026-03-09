
using MediatR;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Commands.Books;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly IDbContext _context;
    private readonly IAuthorRepository _authorRepository;

    public CreateBookCommandHandler(IDbContext context, IAuthorRepository authorRepository)
    {
        _context = context;
        _authorRepository = authorRepository;
    }

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // Create the book
        var book = new Book
        {
            Title = request.Title,
            PublicationDate = request.PublicationDate.HasValue
                ? DateTime.SpecifyKind(request.PublicationDate.Value.Date, DateTimeKind.Utc)
                : null
        };

        _context.Books.Add(book);

        // Handle authors - reuse existing or create new
        foreach (var authorName in request.Authors.Distinct())
        {
            var author = await _authorRepository.GetByNameAsync(authorName);

            if (author == null)
            {
                author = new Author { Name = authorName };
                await _authorRepository.AddAsync(author);
            }

            var bookAuthor = new BookAuthor
            {
                BookId = book.Id,
                AuthorId = author.Id
            };

            _context.BookAuthors.Add(bookAuthor);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Return DTO
        var bookDto = await _context.Books
            .Where(b => b.Id == book.Id)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                PublicationDate = b.PublicationDate,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                Authors = b.BookAuthors.Select(ba => new AuthorDto
                {
                    Id = ba.Author.Id,
                    Name = ba.Author.Name
                }).ToList()
            })
            .FirstAsync(cancellationToken);

        return bookDto;
    }
}
