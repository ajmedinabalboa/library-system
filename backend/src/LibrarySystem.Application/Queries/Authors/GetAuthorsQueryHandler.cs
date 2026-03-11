using MediatR;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;

namespace LibrarySystem.Application.Queries.Authors;

/// <summary>
/// Handles <see cref="GetAuthorsQuery"/> by delegating to the author
/// repository abstraction defined in the Application layer.
///
/// Design patterns applied:
/// - <b>CQRS</b>: read-side query handler, clearly separated from commands.
/// - <b>SOLID – Dependency Inversion Principle (DIP)</b>: depends on the
///   <see cref="IAuthorRepository"/> interface, not on the concrete EF
///   implementation.
/// </summary>
public sealed class GetAuthorsQueryHandler : IRequestHandler<GetAuthorsQuery, List<AuthorDto>>
{
    private readonly IAuthorRepository _authorRepository;

    public GetAuthorsQueryHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<List<AuthorDto>> Handle(
        GetAuthorsQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Search))
        {
            var all = await _authorRepository.GetAllAsync();
            return all
                .OrderBy(a => a.Name)
                .Take(20)
                .Select(a => new AuthorDto { Id = a.Id, Name = a.Name })
                .ToList();
        }

        var authors = await _authorRepository.SearchByNameAsync(request.Search);
        return authors
            .Select(a => new AuthorDto { Id = a.Id, Name = a.Name })
            .ToList();
    }
}
