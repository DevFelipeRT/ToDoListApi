using Microsoft.AspNetCore.Mvc;
using MediatR;
using ToDoApi.Identity.Contracts.Requests;
using ToDoApi.Identity.Contracts.Responses;
using Application.Accounts.Commands.Users;
using Application.Accounts.Services.Interfaces;
using System;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Identity.Client;
using Domain.Accounts;
using Domain.Accounts.ValueObjects;

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
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request?.Email) ||
            string.IsNullOrWhiteSpace(request?.Password))
        {
            return BadRequest(new { message = "All fields are required." });
        }

        AccountEmail emailVo;
        User user;

        try
        {
            emailVo = new AccountEmail(request.Email);
        }
        catch (Exception)
        {
            return Unauthorized(new { message = "Invalid email." });
        }

        try
        {
            var result = await _authenticationService.AuthenticateAsync(emailVo, request.Password, cancellationToken);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid password." });
            }

            user = result;
        }
        catch (Exception)
        {
            return Unauthorized(new { message = "Unregistered account." });
        }

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
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
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

        var command = new CreateUserCommand(
            request.Email,
            request.Username,
            request.Name,
            request.Password
        );

        Guid userId;

        try
        {
            userId = await _mediator.Send(command, cancellationToken);
        }
        catch (Exception ex)
        {
            // Handler throws InvalidOperationException for business rule violations
            return UnprocessableEntity(new { message = ex.Message });
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
