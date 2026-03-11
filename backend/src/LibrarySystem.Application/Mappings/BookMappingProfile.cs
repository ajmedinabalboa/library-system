using AutoMapper;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;

/// <summary>
/// AutoMapper profile that centralises all entity → DTO mappings for books
/// and authors.
///
/// Design patterns applied:
/// - <b>DRY (Don't Repeat Yourself)</b>: replaces four identical manual
///   <c>Select()</c> projections scattered across command handlers with a
///   single, reusable mapping definition.
/// - <b>SOLID – Single Responsibility Principle (SRP)</b>: mapping logic lives
///   in one dedicated place instead of being embedded in business handlers.
/// </summary>
public sealed class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        CreateMap<Author, AuthorDto>();

        CreateMap<Book, BookDto>()
            .ForMember(
                dest => dest.Authors,
                opt => opt.MapFrom(src => src.BookAuthors.Select(ba => ba.Author)));
    }
}
