namespace LibrarySystem.Application.DTOs;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? PublicationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<AuthorDto> Authors { get; set; } = new();
    public int AuthorCount => Authors?.Count ?? 0;
}
