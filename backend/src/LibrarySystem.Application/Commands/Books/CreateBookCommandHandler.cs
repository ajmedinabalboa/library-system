
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Commands.Books;

/// <summary>
/// Handles the <see cref="CreateBookCommand"/>.
///
/// Design patterns applied:
/// - <b>CQRS</b>: command handler with a clear write-side responsibility.
/// - <b>SOLID – SRP</b>: only handles book creation; mapping is delegated to
///   AutoMapper (<see cref="Mappings.BookMappingProfile"/>).
/// - <b>SOLID – DIP</b>: depends on abstractions (<see cref="IDbContext"/>,
///   <see cref="IAuthorRepository"/>, <see cref="IMapper"/>).
/// </summary>
public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly IDbContext _context;
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;

    public CreateBookCommandHandler(
        IDbContext context,
        IAuthorRepository authorRepository,
        IMapper mapper)
    {
        _context = context;
        _authorRepository = authorRepository;
        _mapper = mapper;
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

        // Handle authors – reuse existing or create new (prevents duplicate authors)
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
