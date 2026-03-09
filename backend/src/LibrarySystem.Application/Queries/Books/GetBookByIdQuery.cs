using MediatR;
using LibrarySystem.Application.DTOs;
namespace LibrarySystem.Application.Queries.Books;

public class GetBookByIdQuery : IRequest<BookDto?>
{
    public Guid Id { get; set; }
}
