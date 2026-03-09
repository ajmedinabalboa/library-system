using MediatR;
using LibrarySystem.Application.DTOs;

namespace LibrarySystem.Application.Commands.Books;

public class UpdateBookCommand : IRequest<BookDto>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? PublicationDate { get; set; }
    public List<string> Authors { get; set; } = new();
}
