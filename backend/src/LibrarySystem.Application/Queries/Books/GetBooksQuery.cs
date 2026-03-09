
using LibrarySystem.Application.DTOs;
using MediatR;

namespace LibrarySystem.Application.Queries.Books;

public class GetBooksQuery : IRequest<PaginatedResult<BookDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Title { get; set; }
    public List<string>? Authors { get; set; }
    public DateTime? PublishedAfter { get; set; }
    public DateTime? PublishedBefore { get; set; }
    public int? MinAuthors { get; set; }
    public string? SortBy { get; set; } // title, publicationDate, authorCount
    public string? SortOrder { get; set; } = "asc"; // asc, desc
}
