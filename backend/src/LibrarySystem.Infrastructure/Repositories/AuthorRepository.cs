
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using LibrarySystem.Infrastructure.Persistence;

namespace LibrarySystem.Infrastructure.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Author?> GetByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());
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
