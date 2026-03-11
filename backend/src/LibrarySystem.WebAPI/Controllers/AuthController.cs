using Microsoft.AspNetCore.Mvc;
using MediatR;
using LibrarySystem.Application.DTOs;
using LibrarySystem.Application.Commands.Auth;

namespace LibrarySystem.WebAPI.Controllers;

/// <summary>
/// REST API for authentication (login and token refresh).
///
/// Clean Architecture: dispatches to Application-layer commands via MediatR.
/// Exception handling is delegated to <c>GlobalExceptionHandlerMiddleware</c>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _mediator.Send(new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        });

        return Ok(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _mediator.Send(new RefreshTokenCommand
        {
            RefreshToken = request.RefreshToken
        });

        return Ok(result);
    }
}


