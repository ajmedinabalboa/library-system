
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using LibrarySystem.Infrastructure.Persistence;

namespace LibrarySystem.Infrastructure.Repositories;

/// <summary>
/// Repository for <see cref="Author"/> entities.
/// Extends the generic <see cref="Repository{T}"/> with author-specific
/// queries (Liskov Substitution Principle – concrete type is fully
/// substitutable for <see cref="IAuthorRepository"/>).
/// </summary>
public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Finds an author by exact name using a <b>database-side</b>
    /// case-insensitive comparison (<c>ILIKE</c>), avoiding the previous
    /// pattern of <c>.ToLower()</c> which could evaluate in memory depending
    /// on the EF provider configuration.
    /// </summary>
    public async Task<Author?> GetByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => EF.Functions.ILike(a.Name, name));
    }

    public async Task<List<Author>> GetByNamesAsync(List<string> names)
    {
        var lowerNames = names.Select(n => n.ToLower()).ToList();
        return await _dbSet
            .Where(a => lowerNames.Contains(a.Name.ToLower()))
            .ToListAsync();
    }

    public async Task<List<Author>> SearchByNameAsync(string query)
    {
        return await _dbSet
            .Where(a => EF.Functions.ILike(a.Name, $"%{query}%"))
            .OrderBy(a => a.Name)
            .Take(10)
            .ToListAsync();
    }
}
