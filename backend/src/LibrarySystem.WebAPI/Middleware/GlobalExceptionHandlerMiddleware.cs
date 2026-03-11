using System.Text.Json;
using FluentValidation;
using LibrarySystem.Domain.Exceptions;

namespace LibrarySystem.WebAPI.Middleware;

/// <summary>
/// ASP.NET Core middleware that catches all unhandled exceptions and translates
/// them into structured JSON error responses.
///
/// Design patterns applied:
/// - <b>SOLID – Single Responsibility Principle (SRP)</b>: centralises error
///   handling in one place so that controllers contain <em>only</em> business
///   dispatch logic (no scattered try/catch blocks).
/// - <b>Middleware / Chain-of-Responsibility</b>: sits in the ASP.NET pipeline
///   and wraps every request.
/// - Handles domain-specific exceptions (<see cref="BookNotFoundException"/>,
///   <see cref="InvalidCredentialsException"/>) as well as FluentValidation
///   errors, mapping each to the appropriate HTTP status code.
/// </summary>
public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, body) = exception switch
        {
            ValidationException ve => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse(
                    "Validation failed.",
                    ve.Errors.Select(e => e.ErrorMessage).ToArray())),

            BookNotFoundException nfe => (
                StatusCodes.Status404NotFound,
                new ErrorResponse(nfe.Message)),

            InvalidCredentialsException ice => (
                StatusCodes.Status401Unauthorized,
                new ErrorResponse(ice.Message)),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse("An unexpected error occurred."))
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }

    private sealed record ErrorResponse(string Message, string[]? Errors = null);
}
