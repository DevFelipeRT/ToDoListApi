using System.Collections.Generic;

namespace ToDoApi.Common.Contracts;

/// <summary>
/// Standard API response envelope for consistent output structure.
/// </summary>
/// <typeparam name="T">Type of the data returned by the API.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Data returned by the API.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Informational or error message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// List of error messages, if any.
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// Pagination information, if applicable.
    /// </summary>
    public PageInfo? Pagination { get; set; }
}

