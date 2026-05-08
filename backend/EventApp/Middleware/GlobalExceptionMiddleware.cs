using System.Text.Json;
using EventApp.Exceptions;

namespace EventApp.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var statusCode = ex is AppException appException
            ? appException.StatusCode
            : StatusCodes.Status500InternalServerError;

        context.Response.StatusCode = statusCode;

        var response = new
        {
            message = ex.Message,
            statusCode
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
