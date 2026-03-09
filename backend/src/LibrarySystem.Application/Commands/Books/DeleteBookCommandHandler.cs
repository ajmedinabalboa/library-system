
using MediatR;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.Interfaces;

namespace LibrarySystem.Application.Commands.Books;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, bool>
{
    private readonly IDbContext _context;

    public DeleteBookCommandHandler(IDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book == null)
        {
            return false;
        }

        // Remove book-author relationships
        var bookAuthors = await _context.BookAuthors
            .Where(ba => ba.BookId == request.Id)
            .ToListAsync(cancellationToken);

        foreach (var ba in bookAuthors)
        {
            _context.BookAuthors.Remove(ba);
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
