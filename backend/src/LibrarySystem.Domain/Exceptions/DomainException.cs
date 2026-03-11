namespace LibrarySystem.Domain.Exceptions;

/// <summary>
/// Base class for all domain-specific exceptions.
/// Follows the Single Responsibility Principle (SRP) by providing a shared
/// base that separates domain errors from infrastructure or framework errors.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}
