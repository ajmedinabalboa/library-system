
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Interfaces;

public interface IDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Book> Books { get; }
    DbSet<Author> Authors { get; }
    DbSet<BookAuthor> BookAuthors { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
