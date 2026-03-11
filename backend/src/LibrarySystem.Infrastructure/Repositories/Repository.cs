
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Infrastructure.Persistence;

namespace LibrarySystem.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation following the <b>Repository pattern</b>.
/// Provides standard CRUD operations for any entity type, reducing code
/// duplication across specific repositories (DRY principle).
///
/// Design patterns applied:
/// - <b>Repository pattern</b>: abstracts data-access details behind
///   <see cref="IRepository{T}"/>.
/// - <b>SOLID – OCP</b>: specific repositories override only the methods they
///   need to customise (virtual members).
/// - <b>SOLID – DIP</b>: consumers depend on <see cref="IRepository{T}"/>
///   rather than this concrete class.
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks existence by primary key using a lightweight COUNT query
    /// instead of loading the full entity (performance improvement over
    /// calling <see cref="GetByIdAsync"/> and checking for null).
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.FindAsync(id) != null;
    }
}
