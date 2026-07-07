namespace UserManagementAPI.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    // Hardcoded test tokens (you can load these from config later)
    private static readonly HashSet<string> ValidTokens = new()
    {
        "token123",
        "test-token",
        "dev-token",
        "super-secret",
    };

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authentication"].FirstOrDefault();

        if (authHeader is null || !authHeader.StartsWith("Bearer "))
        {
            await WriteUnauthorized(context, "Missing or invalid Authentication header");
            return;
        }

        var token = authHeader.Substring("Bearer ".Length);

        if (!ValidTokens.Contains(token))
        {
            await WriteUnauthorized(context, "Invalid test token");
            return;
        }

        // Token is valid → continue
        await _next(context);
    }

    private static async Task WriteUnauthorized(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(
            new
            {
                error = "Unauthorized",
                details = message,
                statusCode = 401,
            }
        );
    }
}
