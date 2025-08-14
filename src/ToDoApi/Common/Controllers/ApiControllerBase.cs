using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ToDoApi.Common.Contracts;

namespace ToDoApi.Common.Controllers;

/// <summary>
/// Base controller providing common API behaviors and helpers.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Returns a standardized success response.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    /// <param name="data">The data to return.</param>
    /// <param name="message">Optional message.</param>
    /// <returns>Standardized API response.</returns>
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
    /// Returns a standardized created response.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    /// <param name="data">The data to return.</param>
    /// <param name="location">Resource location URI.</param>
    /// <returns>Standardized API response with 201 status.</returns>
    protected ActionResult<ApiResponse<T>> Created<T>(T data, string? location = null)
    {
        if (!string.IsNullOrEmpty(location))
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
    /// Returns a standardized error response.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    /// <param name="errors">List of error messages.</param>
    /// <param name="message">Optional message.</param>
    /// <returns>Standardized API error response.</returns>
    protected ActionResult<ApiResponse<T>> Error<T>(List<string> errors, string? message = null)
    {
        return BadRequest(new ApiResponse<T>
        {
            Success = false,
            Errors = errors,
            Message = message
        });
    }

    /// <summary>
    /// Extracts the current user's ID from authentication claims.
    /// </summary>
    /// <returns>The GUID representing the current user's ID.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user is not authenticated or ID claim is missing.</exception>
    protected Guid GetCurrentUserId()
    {
        // Check if user is authenticated
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        // Try to get user ID from claims
        var userIdClaim = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            throw new UnauthorizedAccessException("User ID claim is missing");
        }

        // Parse the ID as GUID
        if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            throw new UnauthorizedAccessException("User ID is not a valid GUID");
        }

        return userId;
    }

    /// <summary>
    /// Handles standard API exceptions and returns appropriate responses.
    /// </summary>
    /// <param name="ex">The exception to handle.</param>
    /// <returns>An action result with appropriate error details.</returns>
    protected ActionResult HandleApiException(Exception ex)
    {
        Console.WriteLine(ex.ToString());
        
        if (ex is UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Authentication required",
                Errors = new List<string> { ex.Message }
            });
        }

        // Add other exception types as needed
        
        // Default case - internal server error
        return StatusCode(500, new ApiResponse<object>
        {
            Success = false,
            Message = "An unexpected error occurred",
            //Errors = new List<string> { "Please try again later or contact support" }
            Errors = new List<string> { ex.Message }
        });
    }
}
