using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MediatR;

using Infrastructure.IdentityAccess;
using Application.Accounts.Services.Interfaces;
using Api.Identity.Contracts.Requests;
using Api.Identity.Contracts.Responses;
using Application.Accounts.Queries;
using Application.Accounts.Commands;
using Application.Accounts.DTOs;
using Application.Abstractions.Links;

namespace Api.Identity.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwt;
    private readonly IUrlCrypto _urlCrypto;

    /// <summary>
    /// Initializes a new instance of <see cref="AuthController"/>.
    /// </summary>
    public AuthController(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwt,
        IUrlCrypto urlCrypto)
    {
        _mediator = mediator;
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt;
        _urlCrypto = urlCrypto;
    }

    /// <summary>
    /// Authenticates a user with ASP.NET Identity and returns a JWT on success.
    /// </summary>
    /// <param name="request">Credentials payload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>An <see cref="AuthResponse"/> with access token and account data.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        if (!IsValidLoginRequest(request))
            return BadRequest(new { message = "Email and password are required." });

        var user = await FindUserAsync(request.Email, ct);
        if (user is null)
            return Unauthorized(new { message = "Invalid credentials." });

        if (!await ValidatePasswordAsync(user, request.Password))
            return Unauthorized(new { message = "Invalid credentials." });

        var account = await GetAccountByEmailAsync(user.Email!, ct);
        if (account is null)
            return Unauthorized(new { message = "Account not found." });

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwt.GenerateToken(Guid.Parse(account.Id.ToString()), account.Username, roles);

        return Ok(new AuthResponse
        {
            Token = token,
            AccountId = Guid.Parse(account.Id.ToString()),
            Username = account.Username,
            Name = account.Name,
            Email = account.Email
        });
    }

    /// <summary>
    /// Registers a new account and returns a JWT on success.
    /// </summary>
    /// <param name="request">Registration payload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="RegisterResponse"/> with access token and account data.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        if (!IsValidRegisterRequest(request))
            return BadRequest(new { message = "Name, username, email and password are required." });

        Guid accountId;
        try
        {
            accountId = await _mediator.Send(
                new CreateAccountCommand(request.Email, request.Username, request.Name, request.Password),
                ct);
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { message = ex.Message });
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        var roles = user is null ? Array.Empty<string>() : await _userManager.GetRolesAsync(user);
        var token = _jwt.GenerateToken(accountId, request.Username, roles);

        var response = new RegisterResponse
        {
            Token = token,
            AccountId = accountId,
            Username = request.Username,
            Name = request.Name,
            Email = request.Email
        };

        return Created(string.Empty, response);
    }

    /// <summary>
    /// Confirms email using ASP.NET Identity and activates the domain account.
    /// Expects URL format: GET /api/auth/confirm-email?token={base64url}&uid={protectedUid}
    /// </summary>
    [HttpGet("confirm-email")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> ConfirmEmail(
        [FromQuery] string token,
        [FromQuery] string uid,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(uid))
            return BadRequest(new { message = "Invalid activation link." });

        // 1) Unprotect UID (CredentialId)
        string rawUid;
        try
        {
            rawUid = _urlCrypto.Unprotect(uid);
        }
        catch
        {
            return BadRequest(new { message = "Invalid activation link." });
        }

        // Normalize to Identity's user id (Guid "D" or string as-is)
        var userId = Guid.TryParse(rawUid, out var g) ? g.ToString("D") : rawUid.Trim();

        // 2) Locate user
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Unauthorized(new { message = "Invalid activation link." });

        // 3) Decode Base64Url token back to original format
        string decodedToken;
        try
        {
            var bytes = WebEncoders.Base64UrlDecode(token);
            decodedToken = Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return BadRequest(new { message = "Invalid token format." });
        }

        // 4) Confirm email with Identity
        var confirm = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!confirm.Succeeded)
            return BadRequest(new { message = "Invalid or expired token." });

        // 5) Activate domain account (idempotent no seu command handler)
        var accountDto = await _mediator.Send(new GetAccountByCredentialIdQuery(userId), ct);
        if (accountDto is null)
            return Unauthorized(new { message = "Invalid activation link." });

        if (!accountDto.IsActive)
            await _mediator.Send(new ActivateAccountCommand(accountDto.Id), ct);

        // 6) Issue JWT
        var roles = await _userManager.GetRolesAsync(user);
        var jwt = _jwt.GenerateToken(Guid.Parse(accountDto.Id.ToString()), accountDto.Username, roles);

        return Ok(new AuthResponse
        {
            Token = jwt,
            AccountId = Guid.Parse(accountDto.Id.ToString()),
            Username = accountDto.Username,
            Name = accountDto.Name,
            Email = accountDto.Email
        });
    }

    private static bool IsValidLoginRequest(LoginRequest? request) =>
        request is not null &&
        !string.IsNullOrWhiteSpace(request.Email) &&
        !string.IsNullOrWhiteSpace(request.Password);

    private static bool IsValidRegisterRequest(RegisterRequest? request) =>
        request is not null &&
        !string.IsNullOrWhiteSpace(request.Name) &&
        !string.IsNullOrWhiteSpace(request.Username) &&
        !string.IsNullOrWhiteSpace(request.Email) &&
        !string.IsNullOrWhiteSpace(request.Password);

    private async Task<ApplicationUser?> FindUserAsync(string login, CancellationToken ct)
    {
        var byEmail = await _userManager.FindByEmailAsync(login);
        if (byEmail is not null) return byEmail;
        return await _userManager.FindByNameAsync(login);
    }

    private async Task<bool> ValidatePasswordAsync(ApplicationUser user, string password)
    {
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        return result.Succeeded;
    }

    private Task<AccountDto?> GetAccountByEmailAsync(string email, CancellationToken ct) =>
        _mediator.Send(new GetAccountByEmailQuery(email), ct);
}
