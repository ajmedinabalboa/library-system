namespace LibrarySystem.Domain.Exceptions;

/// <summary>
/// Thrown when authentication fails due to invalid email/password or an
/// expired / revoked refresh token.
/// Replaces the generic <see cref="UnauthorizedAccessException"/> with an
/// expressive, domain-meaningful exception (Clean Code – meaningful names).
/// </summary>
public sealed class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException(string message) : base(message) { }
}
