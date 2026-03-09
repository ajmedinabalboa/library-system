
using LibrarySystem.Domain.Entities;
using LibrarySystem.Application.DTOs;

namespace LibrarySystem.Application.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<PaginatedResult<BookDto>> GetBooksAsync(
        int page,
        int pageSize,
        string? title = null,
        List<string>? authors = null,
        DateTime? publishedAfter = null,
        DateTime? publishedBefore = null,
        int? minAuthors = null,
        string? sortBy = null,
        string? sortOrder = null);

    Task<BookDto?> GetBookByIdAsync(Guid id);
}
