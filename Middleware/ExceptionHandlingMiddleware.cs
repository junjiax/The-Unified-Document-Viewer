using Chap10.Dtos;
using Serilog;
using System.Net;

namespace Chap10.Middleware;

/// <summary>
/// Global exception handling middleware to catch and log all unhandled exceptions
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            _logger.LogInformation("Processing request: {Method} {Path} from {RemoteIp}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress);

            await _next(context);

            _logger.LogInformation("Request completed: {Method} {Path} with status {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred during request processing for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new APIResponse<object>();

        switch (exception)
        {
            case ArgumentNullException argNullEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = APIResponse<object>.Fail($"Invalid argument: {argNullEx.ParamName}", new[] { argNullEx.Message });
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = APIResponse<object>.Fail($"Invalid input: {argEx.Message}", new[] { argEx.Message });
                break;

            case InvalidOperationException invOpEx:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = APIResponse<object>.Fail(invOpEx.Message, new[] { invOpEx.Message });
                break;

            case TimeoutException timeoutEx:
                context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                response = APIResponse<object>.Fail("Request timeout", new[] { timeoutEx.Message });
                break;

            case HttpRequestException httpEx:
                context.Response.StatusCode = StatusCodes.Status502BadGateway;
                response = APIResponse<object>.Fail("External service error", new[] { httpEx.Message });
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = APIResponse<object>.Fail("An unexpected error occurred", new[] { exception.Message });
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}
