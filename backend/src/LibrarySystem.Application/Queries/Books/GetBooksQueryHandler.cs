
using MediatR;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;

namespace LibrarySystem.Application.Queries.Books;

public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, PaginatedResult<BookDto>>
{
    private readonly IBookRepository _bookRepository;

    public GetBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<PaginatedResult<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        return await _bookRepository.GetBooksAsync(
            request.Page,
            request.PageSize,
            request.Title,
            request.Authors,
            request.PublishedAfter,
            request.PublishedBefore,
            request.MinAuthors,
            request.SortBy,
            request.SortOrder);
    }
}
