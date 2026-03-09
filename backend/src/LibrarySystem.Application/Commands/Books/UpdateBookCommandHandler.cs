using MediatR;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Commands.Books;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
{
    private readonly IDbContext _context;
    private readonly IAuthorRepository _authorRepository;

    public UpdateBookCommandHandler(IDbContext context, IAuthorRepository authorRepository)
    {
        _context = context;
        _authorRepository = authorRepository;
    }

    public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .Include(b => b.BookAuthors)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {request.Id} not found");
        }

        // Update book properties
        book.Title = request.Title;
        book.PublicationDate = request.PublicationDate.HasValue
            ? DateTime.SpecifyKind(request.PublicationDate.Value.Date, DateTimeKind.Utc)
            : null;
        book.UpdatedAt = DateTime.UtcNow;

        // Remove existing author relationships
        var existingBookAuthors = await _context.BookAuthors
            .Where(ba => ba.BookId == book.Id)
            .ToListAsync(cancellationToken);

        foreach (var ba in existingBookAuthors)
        {
            _context.BookAuthors.Remove(ba);
        }

        // Add new author relationships
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
