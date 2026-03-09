using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using LibrarySystem.Infrastructure.Persistence;

namespace LibrarySystem.Infrastructure.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PaginatedResult<BookDto>> GetBooksAsync(
        int page,
        int pageSize,
        string? title = null,
        List<string>? authors = null,
        DateTime? publishedAfter = null,
        DateTime? publishedBefore = null,
        int? minAuthors = null,
        string? sortBy = null,
        string? sortOrder = null)
    {
        var query = _context.Books
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(b => EF.Functions.ILike(b.Title, $"%{title}%"));
        }

        if (authors != null && authors.Count > 0)
        {
            var authorPatterns = authors
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => $"%{a}%")
                .ToList();
            if (authorPatterns.Count > 0)
            {
                query = query.Where(b => b.BookAuthors.Any(ba =>
                    authorPatterns.Any(p => EF.Functions.ILike(ba.Author.Name, p))));
            }
        }

        if (publishedAfter.HasValue)
        {
            var afterDate = DateTime.SpecifyKind(publishedAfter.Value.Date, DateTimeKind.Utc);
            query = query.Where(b => b.PublicationDate >= afterDate);
        }

        if (publishedBefore.HasValue)
        {
            // Include the entire "hasta" day by going to end-of-day
            var beforeDate = DateTime.SpecifyKind(publishedBefore.Value.Date.AddDays(1), DateTimeKind.Utc);
            query = query.Where(b => b.PublicationDate < beforeDate);
        }

        if (minAuthors.HasValue)
        {
            query = query.Where(b => b.BookAuthors.Count >= minAuthors.Value);
        }

        // Apply sorting
        query = (sortBy?.ToLower(), sortOrder?.ToLower()) switch
        {
            ("title", "desc") => query.OrderByDescending(b => b.Title),
            ("title", _) => query.OrderBy(b => b.Title),
            ("publicationdate", "desc") => query.OrderByDescending(b => b.PublicationDate),
            ("publicationdate", _) => query.OrderBy(b => b.PublicationDate),
            ("authorcount", "desc") => query.OrderByDescending(b => b.BookAuthors.Count),
            ("authorcount", _) => query.OrderBy(b => b.BookAuthors.Count),
            _ => query.OrderBy(b => b.CreatedAt)
        };

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var books = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
            .ToListAsync();

        return new PaginatedResult<BookDto>
        {
            Items = books,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id)
    {
        return await _context.Books
            .Where(b => b.Id == id)
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
            .FirstOrDefaultAsync();
    }
}
