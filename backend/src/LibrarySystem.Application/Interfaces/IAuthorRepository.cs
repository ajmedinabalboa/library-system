
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> GetByNameAsync(string name);
    Task<List<Author>> GetByNamesAsync(List<string> names);
    Task<List<Author>> SearchByNameAsync(string query);
}
