
using MediatR;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Commands.Auth;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
{
    private readonly IDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(IDbContext context, IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken == null || refreshToken.Revoked || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Revoke old refresh token
        refreshToken.Revoked = true;

        // Generate new tokens
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(refreshToken.User);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = refreshToken.UserId,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 3600
        };
    }
}
