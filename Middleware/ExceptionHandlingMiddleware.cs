using System.Net;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IncidentServiceAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                await WriteProblemDetails(
                    context,
                    HttpStatusCode.NotFound,
                    ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                await WriteProblemDetails(
                    context,
                    HttpStatusCode.BadRequest,
                    ex.Message);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                _logger.LogWarning(ex, "Unique constraint violation.");
                await WriteProblemDetails(
                    context,
                    HttpStatusCode.Conflict,
                    "A record with the same key already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await WriteProblemDetails(
                    context,
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred.");
            }
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlException)
            {
                return sqlException.Number == 2601 || sqlException.Number == 2627;
            }

            return false;
        }

        private static async Task WriteProblemDetails(
            HttpContext context,
            HttpStatusCode statusCode,
            string detail)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                type = "https://httpstatuses.com/" + (int)statusCode,
                title = statusCode.ToString(),
                status = (int)statusCode,
                detail = detail
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}