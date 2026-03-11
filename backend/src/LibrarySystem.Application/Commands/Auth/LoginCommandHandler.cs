using MediatR;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using LibrarySystem.Domain.Exceptions;

namespace LibrarySystem.Application.Commands.Auth;

/// <summary>
/// Handles the <see cref="LoginCommand"/>.
///
/// Design patterns applied:
/// - <b>CQRS</b>: write-side command handler that authenticates a user and
///   issues JWT tokens.
/// - <b>SOLID – DIP</b>: depends on <see cref="IDbContext"/>,
///   <see cref="IJwtTokenGenerator"/>, and <see cref="IPasswordHasher"/>
///   abstractions.
/// - Throws <see cref="InvalidCredentialsException"/> (domain exception) for
///   meaningful error semantics instead of the generic
///   <see cref="UnauthorizedAccessException"/>.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(
        IDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            throw new InvalidCredentialsException("Invalid email or password.");
        }

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600
        };
    }
}
