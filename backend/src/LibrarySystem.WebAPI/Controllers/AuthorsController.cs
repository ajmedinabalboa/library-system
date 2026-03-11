using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Queries.Authors;

namespace LibrarySystem.WebAPI.Controllers;

/// <summary>
/// Exposes author-search endpoints.
///
/// Clean Architecture compliance: the controller dispatches to an Application-
/// layer query (<see cref="GetAuthorsQuery"/>) via MediatR instead of
/// injecting an Infrastructure repository directly. This preserves the
/// dependency rule (outer layers depend on inner layers, never vice-versa).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Search authors by name fragment. Returns up to 10 matches when a
    /// search term is provided, or up to 20 authors when no term is given.
    /// GET /api/authors?search=mart
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<AuthorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<AuthorDto>>> SearchAuthors([FromQuery] string? search = null)
    {
        var result = await _mediator.Send(new GetAuthorsQuery { Search = search });
        return Ok(result);
    }
}

