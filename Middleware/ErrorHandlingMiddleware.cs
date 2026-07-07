using System.Net;
using System.Text.Json;

namespace UserManagementAPI.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception occurred.");

            await WriteErrorResponseAsync(context, ex);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, Exception ex)
    {
        if (!context.Response.HasStarted)
        {
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                error = "An unexpected error occurred.",
                details = ex.Message,
                statusCode = context.Response.StatusCode
            };

            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }
        else
        {
            // Response already started — cannot write JSON body
            // Log and do nothing
        }
    }
}
