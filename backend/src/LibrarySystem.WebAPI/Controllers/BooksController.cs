using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Commands.Books;
using LibrarySystem.Application.Queries.Books;

namespace LibrarySystem.WebAPI.Controllers;

/// <summary>
/// REST API for book management.
///
/// Clean Architecture: all requests are dispatched through MediatR so the
/// controller acts purely as a thin HTTP adapter (SOLID – SRP).
/// Exception handling is delegated to <c>GlobalExceptionHandlerMiddleware</c>
/// (no scattered try/catch blocks needed here).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<BookDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaginatedResult<BookDto>>> GetBooks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? title = null,
        [FromQuery] List<string>? authors = null,
        [FromQuery] DateTime? publishedAfter = null,
        [FromQuery] DateTime? publishedBefore = null,
        [FromQuery] int? minAuthors = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null)
    {
        var query = new GetBooksQuery
        {
            Page = page,
            PageSize = pageSize,
            Title = title,
            Authors = authors,
            PublishedAfter = publishedAfter,
            PublishedBefore = publishedBefore,
            MinAuthors = minAuthors,
            SortBy = sortBy,
            SortOrder = sortOrder
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BookDto>> GetBookById(Guid id)
    {
        var result = await _mediator.Send(new GetBookByIdQuery { Id = id });

        if (result == null)
        {
            return NotFound(new { message = $"Book with ID {id} not found" });
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto dto)
    {
        var command = new CreateBookCommand
        {
            Title = dto.Title,
            PublicationDate = dto.PublicationDate,
            Authors = dto.Authors
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBookById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BookDto>> UpdateBook(Guid id, [FromBody] UpdateBookDto dto)
    {
        var command = new UpdateBookCommand
        {
            Id = id,
            Title = dto.Title,
            PublicationDate = dto.PublicationDate,
            Authors = dto.Authors
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteBook(Guid id)
    {
        var result = await _mediator.Send(new DeleteBookCommand { Id = id });

        if (!result)
        {
            return NotFound(new { message = $"Book with ID {id} not found" });
        }

        return NoContent();
    }
}


