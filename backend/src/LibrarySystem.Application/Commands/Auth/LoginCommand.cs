
using MediatR;
using LibrarySystem.Application.DTOs;

namespace LibrarySystem.Application.Commands.Auth;

public class LoginCommand : IRequest<LoginResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
