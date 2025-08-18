using System;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Domain.Accounts.ValueObjects;
using Application.Accounts.Services.Interfaces;
using Api.Identity.Contracts.Requests;
using Api.Identity.Contracts.Responses;
using Domain.Accounts.Entities;
using Application.Accounts.Commands;

namespace Api.Identity.Controllers;

/// <summary>
/// Provides endpoints for account authentication (login) and account registration.
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
    /// Authenticates the account and returns a JWT token if credentials are valid.
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
        Account account;

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

            account = result;
        }
        catch (Exception)
        {
            return Unauthorized(new { message = "Unregistered account." });
        }

        var token = _jwtTokenService.GenerateToken(account.Id.Value, account.Username.Value);

        var response = new AuthResponse
        {
            Token = token,
            AccountId = account.Id.Value,
            Username = account.Username.Value,
            Name = account.Name.Value,
            Email = account.Email.Value
        };

        return Ok(response);
    }

    /// <summary>
    /// Registers a new account account and returns a JWT token upon successful registration.
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

        var command = new CreateAccountCommand(
            request.Email,
            request.Username,
            request.Name,
            request.Password
        );

        Guid accountId;

        try
        {
            accountId = await _mediator.Send(command, cancellationToken);
        }
        catch (Exception ex)
        {
            // Handler throws InvalidOperationException for business rule violations
            return UnprocessableEntity(new { message = ex.Message });
        }

        var token = _jwtTokenService.GenerateToken(accountId, request.Username);

        var response = new RegisterResponse
        {
            Token = token,
            AccountId = accountId,
            Username = request.Username,
            Name = request.Name,
            Email = request.Email
        };

        return CreatedAtAction(nameof(Register), response);
    }
}
