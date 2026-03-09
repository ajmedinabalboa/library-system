using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Commands.Books;
using LibrarySystem.Application.Queries.Books;

namespace LibrarySystem.WebAPI.Controllers;

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
    public async Task<ActionResult<BookDto>> GetBookById(Guid id)
    {
        var query = new GetBookByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound(new { message = $"Book with ID {id} not found" });
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto dto)
    {
        try
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
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BookDto>> UpdateBook(Guid id, [FromBody] UpdateBookDto dto)
    {
        try
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
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteBook(Guid id)
    {
        try
        {
            var command = new DeleteBookCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound(new { message = $"Book with ID {id} not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

