using Microsoft.AspNetCore.Mvc;
using MediatR;
using ToDoApi.Identity.Contracts.Requests;
using ToDoApi.Identity.Contracts.Responses;
using Application.Accounts.Commands.Users;
using Application.Accounts.Services.Interfaces;
using System;

namespace ToDoApi.Identity.Controllers;

/// <summary>
/// Provides endpoints for user authentication (login) and user registration.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthenticationService _authenticationService;
    private readonly IJwtTokenService _jwtTokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    public AuthController(
        IMediator mediator,
        IAuthenticationService authenticationService,
        IJwtTokenService jwtTokenService)
    {
        _mediator = mediator;
        _authenticationService = authenticationService;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Authenticates the user and returns a JWT token if credentials are valid.
    /// All failure cases (invalid credentials, inactive account, lockout) result in HTTP 401.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.Password))
            return Unauthorized(new { message = "Invalid credentials." });

        var emailVo = new Domain.Accounts.ValueObjects.AccountEmail(request.Email);

        var user = await _authenticationService.AuthenticateAsync(emailVo, request.Password, cancellationToken);

        if (user is null)
            return Unauthorized(new { message = "Invalid credentials." });

        var token = _jwtTokenService.GenerateToken(user.Id.Value, user.Username.Value);

        var response = new AuthResponse
        {
            Token = token,
            UserId = user.Id.Value,
            Username = user.Username.Value,
            Name = user.Name.Value,
            Email = user.Email.Value
        };

        return Ok(response);
    }

    /// <summary>
    /// Registers a new user account and returns a JWT token upon successful registration.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request?.Email) ||
            string.IsNullOrWhiteSpace(request?.Password) ||
            string.IsNullOrWhiteSpace(request?.Username) ||
            string.IsNullOrWhiteSpace(request?.Name))
        {
            return BadRequest(new { message = "All fields are required." });
        }

        var userId = Guid.NewGuid();

        var command = new CreateUserCommand(
            userId,
            request.Email,
            request.Username,
            request.Name,
            request.Password
        );

        try
        {
            await _mediator.Send(command, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            // Handler throws InvalidOperationException for business rule violations
            return BadRequest(new { message = ex.Message });
        }

        var token = _jwtTokenService.GenerateToken(userId, request.Username);

        var response = new RegisterResponse
        {
            Token = token,
            UserId = userId,
            Username = request.Username,
            Name = request.Name,
            Email = request.Email
        };

        return CreatedAtAction(nameof(Register), response);
    }
}
