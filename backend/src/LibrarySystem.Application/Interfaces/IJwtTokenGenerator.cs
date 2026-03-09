
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
