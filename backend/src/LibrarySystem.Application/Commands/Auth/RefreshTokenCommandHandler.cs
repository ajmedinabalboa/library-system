
using MediatR;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using LibrarySystem.Domain.Exceptions;

namespace LibrarySystem.Application.Commands.Auth;

/// <summary>
/// Handles the <see cref="RefreshTokenCommand"/> by rotating the refresh token
/// (revoke old → issue new), following the token-rotation security pattern.
///
/// Design patterns applied:
/// - <b>CQRS</b>: write-side command handler.
/// - <b>SOLID – DIP</b>: depends on <see cref="IDbContext"/> and
///   <see cref="IJwtTokenGenerator"/> abstractions.
/// - Throws <see cref="InvalidCredentialsException"/> (domain exception) for
///   meaningful error semantics.
/// </summary>
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
            throw new InvalidCredentialsException("Invalid or expired refresh token.");
        }

        // Revoke old refresh token (token-rotation pattern)
        refreshToken.Revoked = true;

        // Issue new tokens
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(refreshToken.User);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = refreshToken.UserId,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 3600
        };
    }
}
