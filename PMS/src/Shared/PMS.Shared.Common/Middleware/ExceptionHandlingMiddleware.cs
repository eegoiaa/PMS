using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using PMS.Shared.Common.Exceptions;

namespace PMS.Shared.Common.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IWebHostEnvironment env)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var traceId = Guid.NewGuid().ToString();
            var endpoint = context.GetEndpoint();
            var technicalAction = endpoint?.DisplayName ?? $"{context.Request.Method} {context.Request.Path}";
            var publicAction = $"{context.Request.Method} {context.Request.Path}";

            _logger.LogError(ex, "Error ID: {TraceId}. Technical Action: {Action}. Message: {Message}",
                traceId, technicalAction, ex.Message);

            var (statusCode, safeMessage) = ex switch
            {
                OperationCanceledException => (409, $"The request for '{publicAction}' has been cancelled."),
                UnauthorizedAccessException => (401, "Access is denied"),
                BaseBusinessException businessEx => (businessEx.StatusCode, businessEx.Message),
                FluentValidation.ValidationException valEx => (400, string.Join("; ", valEx.Errors.Select(e => e.ErrorMessage))),
                _ => (500, $"An error occurred while performing an action: {publicAction}")
            };

            await HandleExceptionAsync(context, statusCode, safeMessage, traceId, env, ex);
        }
    }

    public static Task HandleExceptionAsync(
        HttpContext context,
        int statusCode,
        string message,
        string traceId,
        IWebHostEnvironment env,
        Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            StatusCode = statusCode,
            Message = message,
            TraceId = traceId,
            Details = env.IsDevelopment() ? ex.ToString() : null
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
