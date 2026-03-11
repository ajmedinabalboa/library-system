using MediatR;
using LibrarySystem.Application.DTOs;

namespace LibrarySystem.Application.Queries.Authors;

/// <summary>
/// Query that retrieves authors, optionally filtered by a name fragment.
///
/// By exposing authors through a CQRS query instead of directly from the
/// controller, this keeps the <b>Clean Architecture</b> dependency rule intact:
/// the WebAPI layer depends only on the Application layer, never on
/// Infrastructure interfaces.
/// </summary>
public class GetAuthorsQuery : IRequest<List<AuthorDto>>
{
    /// <summary>Optional name fragment for a case-insensitive search.</summary>
    public string? Search { get; set; }
}
