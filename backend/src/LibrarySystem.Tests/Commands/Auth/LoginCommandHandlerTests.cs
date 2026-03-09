using LibrarySystem.Application.Commands.Auth;

namespace LibrarySystem.Tests.Commands.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IJwtTokenGenerator> _mockJwtGenerator = new();
    private readonly Mock<IPasswordHasher> _mockPasswordHasher = new();

    private LoginCommandHandler CreateHandler(TestDbContext context) =>
        new(context, _mockJwtGenerator.Object, _mockPasswordHasher.Object);

    private static User CreateUser(string email = "user@example.com", string hash = "hashed_password") =>
        new() { Email = email, PasswordHash = hash, Name = "Test User", Role = "User" };

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResponse()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        _mockPasswordHasher.Setup(h => h.VerifyPassword("hashed_password", "correct_password")).Returns(true);
        _mockJwtGenerator.Setup(g => g.GenerateAccessToken(It.IsAny<User>())).Returns("access_token");
        _mockJwtGenerator.Setup(g => g.GenerateRefreshToken()).Returns("refresh_token");

        var command = new LoginCommand { Email = "user@example.com", Password = "correct_password" };

        // Act
        var result = await CreateHandler(context).Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("access_token", result.AccessToken);
        Assert.Equal("refresh_token", result.RefreshToken);
        Assert.Equal(3600, result.ExpiresIn);
    }

    [Fact]
    public async Task Handle_ValidCredentials_PersistsRefreshTokenToDatabase()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        _mockPasswordHasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _mockJwtGenerator.Setup(g => g.GenerateAccessToken(It.IsAny<User>())).Returns("at");
        _mockJwtGenerator.Setup(g => g.GenerateRefreshToken()).Returns("stored_rt");

        var command = new LoginCommand { Email = "user@example.com", Password = "any" };

        // Act
        await CreateHandler(context).Handle(command, CancellationToken.None);

        // Assert
        var stored = context.RefreshTokens.FirstOrDefault(rt => rt.Token == "stored_rt");
        Assert.NotNull(stored);
        Assert.False(stored.Revoked);
        Assert.Equal(user.Id, stored.UserId);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var command = new LoginCommand { Email = "ghost@example.com", Password = "password" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => CreateHandler(context).Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        _mockPasswordHasher.Setup(h => h.VerifyPassword("hashed_password", "wrong")).Returns(false);

        var command = new LoginCommand { Email = "user@example.com", Password = "wrong" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => CreateHandler(context).Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongPassword_DoesNotGenerateToken()
    {
        // Arrange
        await using var context = TestDbContext.Create();
        var user = CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        _mockPasswordHasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var command = new LoginCommand { Email = "user@example.com", Password = "wrong" };

        // Act
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => CreateHandler(context).Handle(command, CancellationToken.None));

        // Assert
        _mockJwtGenerator.Verify(g => g.GenerateAccessToken(It.IsAny<User>()), Times.Never);
        _mockJwtGenerator.Verify(g => g.GenerateRefreshToken(), Times.Never);
    }
}
