using LibrarySystem.Application.DTOs;

namespace LibrarySystem.Tests.DTOs;

public class CreateBookDtoTests
{
    [Fact]
    public void CreateBookDto_DefaultValues_AreInitialized()
    {
        var dto = new CreateBookDto();
        Assert.Equal(string.Empty, dto.Title);
        Assert.Null(dto.PublicationDate);
        Assert.NotNull(dto.Authors);
        Assert.Empty(dto.Authors);
    }

    [Fact]
    public void CreateBookDto_CanSetAllProperties()
    {
        var date = new DateTime(2024, 1, 15);
        var dto = new CreateBookDto
        {
            Title = "Clean Code",
            PublicationDate = date,
            Authors = new List<string> { "Robert Martin" }
        };

        Assert.Equal("Clean Code", dto.Title);
        Assert.Equal(date, dto.PublicationDate);
        Assert.Single(dto.Authors, "Robert Martin");
    }
}

public class UpdateBookDtoTests
{
    [Fact]
    public void UpdateBookDto_DefaultValues_AreInitialized()
    {
        var dto = new UpdateBookDto();
        Assert.Equal(string.Empty, dto.Title);
        Assert.Null(dto.PublicationDate);
        Assert.NotNull(dto.Authors);
        Assert.Empty(dto.Authors);
    }

    [Fact]
    public void UpdateBookDto_CanSetAllProperties()
    {
        var date = new DateTime(2020, 6, 1);
        var dto = new UpdateBookDto
        {
            Title = "The Pragmatic Programmer",
            PublicationDate = date,
            Authors = new List<string> { "David Thomas", "Andrew Hunt" }
        };

        Assert.Equal("The Pragmatic Programmer", dto.Title);
        Assert.Equal(date, dto.PublicationDate);
        Assert.Equal(2, dto.Authors.Count);
    }
}

public class LoginRequestDtoTests
{
    [Fact]
    public void LoginRequestDto_DefaultValues_AreInitialized()
    {
        var dto = new LoginRequestDto();
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.Password);
    }

    [Fact]
    public void LoginRequestDto_CanSetAllProperties()
    {
        var dto = new LoginRequestDto
        {
            Email = "admin@library.com",
            Password = "secret123"
        };

        Assert.Equal("admin@library.com", dto.Email);
        Assert.Equal("secret123", dto.Password);
    }
}

public class RefreshTokenRequestDtoTests
{
    [Fact]
    public void RefreshTokenRequestDto_DefaultValue_IsInitialized()
    {
        var dto = new RefreshTokenRequestDto();
        Assert.Equal(string.Empty, dto.RefreshToken);
    }

    [Fact]
    public void RefreshTokenRequestDto_CanSetToken()
    {
        var dto = new RefreshTokenRequestDto { RefreshToken = "my-refresh-token" };
        Assert.Equal("my-refresh-token", dto.RefreshToken);
    }
}
