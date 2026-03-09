
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Infrastructure.Persistence;

namespace LibrarySystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IBookRepository? _books;
    private IAuthorRepository? _authors;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IBookRepository Books => _books ??= new BookRepository(_context);
    public IAuthorRepository Authors => _authors ??= new AuthorRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
