using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Api.Common.Contracts;

namespace Api.Common.Controllers;

/// <summary>
/// Base controller providing common API behaviors and helpers.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Returns a standardized success response.
    /// </summary>
    [NonAction]
    protected ActionResult<ApiResponse<T>> Success<T>(T data, string? message = null)
    {
        return Ok(new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        });
    }

    /// <summary>
    /// Returns a standardized created response (201) and optionally sets the Location header.
    /// </summary>
    [NonAction]
    protected ActionResult<ApiResponse<T>> Created<T>(T data, string? location = null)
    {
        if (!string.IsNullOrWhiteSpace(location))
        {
            Response.Headers.Append("Location", location);
        }

        return StatusCode(201, new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = "Resource created successfully"
        });
    }

    /// <summary>
    /// Returns a standardized bad request response (400).
    /// </summary>
    [NonAction]
    protected ActionResult<ApiResponse<T>> Error<T>(IEnumerable<string> errors, string? message = null)
    {
        var list = errors?.ToList() ?? new List<string> { "Unknown error" };

        return BadRequest(new ApiResponse<T>
        {
            Success = false,
            Errors = list,
            Message = message
        });
    }

    /// <summary>
    /// Extracts the current account identifier (Guid) from the authenticated user's claims.
    /// Searches common claim types: NameIdentifier, "sub", "oid", and "account_id".
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Thrown when not authenticated or when the identifier is missing/invalid.</exception>
    [NonAction]
    protected Guid GetCurrentAccountId()
    {
        if (User?.Identity?.IsAuthenticated != true)
            throw new UnauthorizedAccessException("Authentication is required.");

        var claim = User.FindFirst(c =>
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type == "sub" ||
            c.Type == "oid" ||
            c.Type == "account_id");

        if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
            throw new UnauthorizedAccessException("Account identifier claim is missing.");

        if (!Guid.TryParse(claim.Value, out var accountId))
            throw new UnauthorizedAccessException("Account identifier claim is not a valid GUID.");

        return accountId;
    }

    /// <summary>
    /// Handles standard API exceptions and returns appropriate HTTP responses.
    /// Extend this method as needed for application-specific exception types.
    /// </summary>
    [NonAction]
    protected ActionResult HandleApiException(Exception ex)
    {
        // Log as appropriate in your application layer/middleware.
        // This base class avoids taking a logging dependency.

        if (ex is UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Authentication required.",
                Errors = new List<string> { ex.Message }
            });
        }

        // Extend with more exception mappings (e.g., domain/application exceptions).

        return StatusCode(500, new ApiResponse<object>
        {
            Success = false,
            Message = "An unexpected error occurred.",
            Errors = new List<string> { ex.Message }
        });
    }
}
