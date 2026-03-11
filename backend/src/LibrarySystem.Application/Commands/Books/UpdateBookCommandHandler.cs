using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using LibrarySystem.Domain.Exceptions;

namespace LibrarySystem.Application.Commands.Books;

/// <summary>
/// Handles the <see cref="UpdateBookCommand"/>.
///
/// Design patterns applied:
/// - <b>CQRS</b>: command handler with a clear write-side responsibility.
/// - <b>SOLID – SRP</b>: handles only book updates; mapping is delegated to
///   AutoMapper (<see cref="Mappings.BookMappingProfile"/>).
/// - <b>SOLID – DIP</b>: depends on abstractions (<see cref="IDbContext"/>,
///   <see cref="IAuthorRepository"/>, <see cref="IMapper"/>).
/// - Throws <see cref="BookNotFoundException"/> (domain exception) instead of
///   the generic <see cref="KeyNotFoundException"/> for meaningful error semantics.
/// </summary>
public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
{
    private readonly IDbContext _context;
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;

    public UpdateBookCommandHandler(
        IDbContext context,
        IAuthorRepository authorRepository,
        IMapper mapper)
    {
        _context = context;
        _authorRepository = authorRepository;
        _mapper = mapper;
    }

    public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .Include(b => b.BookAuthors)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book == null)
        {
            throw new BookNotFoundException(request.Id);
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

            _context.BookAuthors.Add(new BookAuthor
            {
                BookId = book.Id,
                AuthorId = author.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Project directly in the DB query using AutoMapper (DRY – replaces manual Select)
        return await _context.Books
            .Where(b => b.Id == book.Id)
            .ProjectTo<BookDto>(_mapper.ConfigurationProvider)
            .FirstAsync(cancellationToken);
    }
}
