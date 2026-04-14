using System.Net;
using System.Text.Json;
using BudgetFlow.Application.Common.Exceptions;

namespace BudgetFlow.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync (HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message, errors) = exception switch
            {
                ValidationException ex => (
                    HttpStatusCode.BadRequest,
                    "Validation failed",
                    ex.Errors),

                NotFoundException ex => (
                    HttpStatusCode.NotFound,
                    ex.Message,
                    null as IDictionary<string, string[]>),

                ForbiddenException ex => (
                    HttpStatusCode.Forbidden,
                    ex.Message,
                    null as IDictionary<string, string[]>),

                _ => (
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred",
                    null as IDictionary<string, string[]>)
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                status = (int)statusCode,
                message,
                errors
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}