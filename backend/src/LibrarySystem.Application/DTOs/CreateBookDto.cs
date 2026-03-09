namespace LibrarySystem.Application.DTOs;

public class CreateBookDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime? PublicationDate { get; set; }
    public List<string> Authors { get; set; } = new();
}
