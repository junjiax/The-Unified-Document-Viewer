using Serilog.Context;
using System.Diagnostics;

namespace Chap10.Middleware;

/// <summary>
/// Middleware to log HTTP request and response details
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.TraceIdentifier;
        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("UserId", context.User?.Identity?.Name ?? "Anonymous"))
        {
            var stopwatch = Stopwatch.StartNew();

            // Log request details
            var request = context.Request;
            _logger.LogInformation(
                "HTTP {Method} request initiated: {Path}{Query} | ClientIP: {ClientIp} | RequestId: {RequestId}",
                request.Method,
                request.Path,
                request.QueryString,
                context.Connection.RemoteIpAddress,
                requestId);

            // Store original response stream
            var originalResponseBody = context.Response.Body;
            using (var responseBodyStream = new MemoryStream())
            {
                context.Response.Body = responseBodyStream;

                try
                {
                    await _next(context);

                    stopwatch.Stop();

                    // Log response details
                    _logger.LogInformation(
                        "HTTP response completed: {Method} {Path} | StatusCode: {StatusCode} | Duration: {DurationMs}ms | RequestId: {RequestId}",
                        request.Method,
                        request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds,
                        requestId);

                    // Copy the response body to the original stream
                    await responseBodyStream.CopyToAsync(originalResponseBody);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _logger.LogError(ex,
                        "HTTP request failed: {Method} {Path} | Duration: {DurationMs}ms | RequestId: {RequestId}",
                        request.Method,
                        request.Path,
                        stopwatch.ElapsedMilliseconds,
                        requestId);
                    throw;
                }
                finally
                {
                    context.Response.Body = originalResponseBody;
                }
            }
        }
    }
}
