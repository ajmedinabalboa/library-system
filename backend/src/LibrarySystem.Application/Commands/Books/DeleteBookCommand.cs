using MediatR;

namespace LibrarySystem.Application.Commands.Books;

public class DeleteBookCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}