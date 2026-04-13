using Auth.API.Application.DTOs;
using Auth.API.Application.Features.Auth.Commands.Login;
using Auth.API.Application.Features.Auth.Commands.RefreshToken;
using Auth.API.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Register a new user</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthTokensDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(Register), result);
    }

    /// <summary>Login with email and password</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthTokensDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    /// <summary>Refresh access token</summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthTokensDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }
}