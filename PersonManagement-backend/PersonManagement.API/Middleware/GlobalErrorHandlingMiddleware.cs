using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace PersonManagement.API.Middleware
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalErrorHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
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
            // Log the exception
            _logger.LogError(exception, "An unhandled exception occurred");

            // Set response details
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                NotFoundException => new ErrorResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = exception.Message,
                    TraceId = context.TraceIdentifier
                },
                ValidationException validationEx => new ErrorResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationEx.Errors,
                    TraceId = context.TraceIdentifier
                },
                UnauthorizedException => new ErrorResponse
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Unauthorized access",
                    TraceId = context.TraceIdentifier
                },
                ForbiddenException => new ErrorResponse
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "Access forbidden",
                    TraceId = context.TraceIdentifier
                },
                _ => new ErrorResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = _environment.IsDevelopment()
                        ? exception.Message
                        : "An error occurred while processing your request.",
                    Details = _environment.IsDevelopment() ? exception.StackTrace : null,
                    TraceId = context.TraceIdentifier
                }
            };

            context.Response.StatusCode = response.StatusCode;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }

    // Custom Exceptions
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "Unauthorized") : base(message) { }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "Forbidden") : base(message) { }
    }

    // Error Response Model
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
        public string TraceId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    // Extension method
    public static class GlobalErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalErrorHandlingMiddleware>();
        }
    }
}