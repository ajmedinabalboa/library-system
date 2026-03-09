namespace LibrarySystem.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public DateTime? PublicationDate { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
