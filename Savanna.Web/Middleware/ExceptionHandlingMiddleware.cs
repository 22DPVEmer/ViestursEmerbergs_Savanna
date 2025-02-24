using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Savanna.Services.Exceptions;

namespace Savanna.Web.Middleware
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse();

            switch (exception)
            {
                case GameServiceException gameEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Game service error";
                    response.Details = gameEx.Message;
                    response.UserId = gameEx.UserId;
                    response.GameId = gameEx.GameId;
                    break;

                case ConfigurationException configEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Configuration error";
                    response.Details = configEx.Message;
                    response.ConfigFile = configEx.ConfigFile;
                    break;

                case FileNotFoundException fileEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "File not found";
                    response.Details = fileEx.Message;
                    break;

                case JsonException jsonEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Invalid JSON format";
                    response.Details = jsonEx.Message;
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Unauthorized access";
                    response.Details = "You do not have permission to perform this action";
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "An error occurred while processing your request";
                    response.Details = "Please try again later";
                    break;
            }

            var result = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(result);
        }

        private class ErrorResponse
        {
            public string Message { get; set; } = string.Empty;
            public string Details { get; set; } = string.Empty;
            public string? ConfigFile { get; set; }
            public string? UserId { get; set; }
            public string? GameId { get; set; }
        }
    }

    // Extension method to make it easier to add the middleware
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
} 