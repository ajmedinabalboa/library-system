using LibrarySystem.Application.Commands.Auth;
using LibrarySystem.Domain.Exceptions;

namespace LibrarySystem.Tests.Commands.Auth;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IJwtTokenGenerator> _mockJwtGenerator = new();

    private RefreshTokenCommandHandler CreateHandler(TestDbContext context) =>
        new(context, _mockJwtGenerator.Object);

    private static async Task<(TestDbContext context, User user, RefreshToken token)> SeedTokenAsync(
        string tokenValue, bool revoked = false, int expiresInDays = 7)
    {
        var context = TestDbContext.Create();
        var user = new User { Email = "user@example.com", Name = "Test", PasswordHash = "hash" };
        context.Users.Add(user);

        var token = new RefreshToken
        {
            UserId = user.Id,
            Token = tokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(expiresInDays),
            Revoked = revoked
        };
        context.RefreshTokens.Add(token);
        await context.SaveChangesAsync();

        return (context, user, token);
    }

    [Fact]
    public async Task Handle_ValidToken_ReturnsNewAccessAndRefreshTokens()
    {
        // Arrange
        var (context, _, _) = await SeedTokenAsync("valid_rt");
        await using var _ = context;

        _mockJwtGenerator.Setup(g => g.GenerateAccessToken(It.IsAny<User>())).Returns("new_at");
        _mockJwtGenerator.Setup(g => g.GenerateRefreshToken()).Returns("new_rt");

        var command = new RefreshTokenCommand { RefreshToken = "valid_rt" };

        // Act
        var result = await CreateHandler(context).Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("new_at", result.AccessToken);
        Assert.Equal("new_rt", result.RefreshToken);
        Assert.Equal(3600, result.ExpiresIn);
    }

    [Fact]
    public async Task Handle_ValidToken_RevokesOldToken()
    {
        // Arrange
        var (context, _, token) = await SeedTokenAsync("old_rt");
        await using var _ = context;

        _mockJwtGenerator.Setup(g => g.GenerateAccessToken(It.IsAny<User>())).Returns("new_at");
        _mockJwtGenerator.Setup(g => g.GenerateRefreshToken()).Returns("brand_new_rt");

        var command = new RefreshTokenCommand { RefreshToken = "old_rt" };

        // Act
        await CreateHandler(context).Handle(command, CancellationToken.None);

        // Assert
        var oldToken = context.RefreshTokens.First(rt => rt.Token == "old_rt");
        Assert.True(oldToken.Revoked);
    }

    [Fact]
    public async Task Handle_ValidToken_PersistsNewRefreshToken()
    {
        // Arrange
        var (context, user, _) = await SeedTokenAsync("valid_rt");
        await using var _ = context;

        _mockJwtGenerator.Setup(g => g.GenerateAccessToken(It.IsAny<User>())).Returns("at");
        _mockJwtGenerator.Setup(g => g.GenerateRefreshToken()).Returns("persisted_new_rt");

        var command = new RefreshTokenCommand { RefreshToken = "valid_rt" };

        // Act
        await CreateHandler(context).Handle(command, CancellationToken.None);

        // Assert
        var newToken = context.RefreshTokens.FirstOrDefault(rt => rt.Token == "persisted_new_rt");
        Assert.NotNull(newToken);
        Assert.False(newToken.Revoked);
        Assert.Equal(user.Id, newToken.UserId);
    }

    [Fact]
    public async Task Handle_RevokedToken_ThrowsInvalidCredentialsException()
    {
        // Arrange
        var (context, _, _) = await SeedTokenAsync("revoked_rt", revoked: true);
        await using var _ = context;

        var command = new RefreshTokenCommand { RefreshToken = "revoked_rt" };

        // Act & Assert – domain exception replaces generic UnauthorizedAccessException
        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => CreateHandler(context).Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ExpiredToken_ThrowsInvalidCredentialsException()
    {
        // Arrange
        var (context, _, _) = await SeedTokenAsync("expired_rt", expiresInDays: -1);
        await using var _ = context;

        var command = new RefreshTokenCommand { RefreshToken = "expired_rt" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => CreateHandler(context).Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NonExistentToken_ThrowsInvalidCredentialsException()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var command = new RefreshTokenCommand { RefreshToken = "does_not_exist" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => CreateHandler(context).Handle(command, CancellationToken.None));
    }
}
