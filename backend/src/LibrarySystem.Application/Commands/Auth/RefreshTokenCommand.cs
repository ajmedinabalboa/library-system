using LibrarySystem.Application.DTOs;
using MediatR;

namespace LibrarySystem.Application.Commands.Auth;

public class RefreshTokenCommand : IRequest<LoginResponseDto>
{
    public string RefreshToken { get; set; } = string.Empty;
}