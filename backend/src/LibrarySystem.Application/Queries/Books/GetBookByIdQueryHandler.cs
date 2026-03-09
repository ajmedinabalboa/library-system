
using MediatR;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;

namespace LibrarySystem.Application.Queries.Books;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIdQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        return await _bookRepository.GetBookByIdAsync(request.Id);
    }
}
