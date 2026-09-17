using Revo.API.Resposes;

namespace Revo.API.Extentions
{
    public static class StatusCodeMiddlewareExtension
    {
        public static IApplicationBuilder UseCustomStatusCodePages(this IApplicationBuilder app)
        {
            return app.UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;

                if (response.StatusCode >= 400 && response.StatusCode < 600 && !response.HasStarted)
                {
                    response.ContentType = "application/json";

                    var message = response.StatusCode switch
                    {
                        404 => "The requested resource was not found.",
                        401 => "You are not authorized to access this resource.",
                        403 => "You do not have permission to access this resource.",
                        _ => "An error occurred while processing your request."
                    };

                    var apiResponse = ApiResponse<object>.Failure(response.StatusCode, "Error", message);
                    await response.WriteAsJsonAsync(apiResponse);
                }
            });
        }
    }
}
