using Hospital.Application.Common;
using Hospital.Application.Common.Exceptions;
using Hospital.Domain.Exceptions;

namespace API_Hospital.Middlewares
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
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                ValidationException => (StatusCodes.Status400BadRequest, exception.Message),
                NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                DomainException => (StatusCodes.Status409Conflict, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Error no controlado en {Method} {Path}", context.Request.Method, context.Request.Path);
            else
                _logger.LogWarning("{Method} {Path} con {StatusCode}. {Type}: {Message}",
                    context.Request.Method, context.Request.Path, statusCode, exception.GetType().Name, exception.Message);

            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new DtoResponseData { Message = message });
        }
    }
}
