namespace LibrarySystem.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "User";

    // Navigation property
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
