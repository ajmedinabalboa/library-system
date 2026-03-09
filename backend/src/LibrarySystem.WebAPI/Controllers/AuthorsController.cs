using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Application.DTOs;

namespace LibrarySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorsController(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    /// <summary>
    /// Search authors by name fragment. Returns up to 10 matches.
    /// GET /api/authors?search=mart
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<AuthorDto>>> SearchAuthors([FromQuery] string? search = null)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            var all = await _authorRepository.GetAllAsync();
            return Ok(all.OrderBy(a => a.Name).Take(20).Select(a => new AuthorDto { Id = a.Id, Name = a.Name }));
        }

        var authors = await _authorRepository.SearchByNameAsync(search);
        return Ok(authors.Select(a => new AuthorDto { Id = a.Id, Name = a.Name }));
    }
}
