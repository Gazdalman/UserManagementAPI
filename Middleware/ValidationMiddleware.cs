using System.Text.Json;
using UserManagementAPI.Model;

namespace UserManagementAPI.Middleware;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (
            context.Request.Method is "POST" or "PUT"
            && context.Request.ContentType?.Contains("application/json") == true
        )
        {
            context.Request.EnableBuffering();

            var user = await JsonSerializer.DeserializeAsync<User>(
                context.Request.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            context.Request.Body.Position = 0;

            var errors = ValidateUser(user);

            if (errors.Count > 0)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { errors });
                return;
            }
        }

        await _next(context);
    }

    private Dictionary<string, string[]> ValidateUser(User? user)
    {
        var errors = new Dictionary<string, string[]>();

        if (user is null)
        {
            errors["Body"] = new[] { "Request body is required." };
            return errors;
        }

        if (string.IsNullOrWhiteSpace(user.FirstName))
            errors["FirstName"] = new[] { "First name is required." };

        if (string.IsNullOrWhiteSpace(user.LastName))
            errors["LastName"] = new[] { "Last name is required." };

        if (string.IsNullOrWhiteSpace(user.Email))
            errors["Email"] = new[] { "Email is required." };
        else if (!user.Email.Contains("@"))
            errors["Email"] = new[] { "Email must be a valid email address." };

        return errors;
    }
}
