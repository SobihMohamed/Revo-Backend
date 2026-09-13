using Microsoft.AspNetCore.Diagnostics;
using Revo.API.Resposes;

namespace Revo.API.GlobalHandler
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IWebHostEnvironment _env;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // 1 - Log the exception
            _logger.LogError(exception, $"An unhandled exception occurred. {exception.Message}");

            // 2 - Set the response status code and content type
            var statusCode = StatusCodes.Status500InternalServerError;

            // 3 - error msg
            var errorMessage = _env.IsDevelopment()
                ? $"{exception.Message} \n {exception.StackTrace}"
                : "An unexpected error occurred on the server. Please try again later.";

            var response = ApiResponse<object>.Failure(statusCode, "Server Error", errorMessage);

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }
    }
}
