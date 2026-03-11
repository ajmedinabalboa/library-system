namespace LibrarySystem.Domain.Exceptions;

/// <summary>
/// Thrown when a requested book does not exist in the system.
/// Replaces the generic <see cref="System.Collections.Generic.KeyNotFoundException"/>
/// with an expressive, domain-meaningful exception (Clean Code – meaningful names).
/// </summary>
public sealed class BookNotFoundException : DomainException
{
    public Guid BookId { get; }

    public BookNotFoundException(Guid bookId)
        : base($"Book with ID '{bookId}' was not found.")
    {
        BookId = bookId;
    }
}
