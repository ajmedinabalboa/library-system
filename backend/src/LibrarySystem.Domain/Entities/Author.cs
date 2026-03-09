namespace LibrarySystem.Domain.Entities;

public class Author : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    // Navigation property
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
