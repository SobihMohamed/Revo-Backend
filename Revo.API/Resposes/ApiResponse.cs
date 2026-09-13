namespace Revo.API.Resposes
{
    public class ApiResponse<TData>
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public TData? Data { get; set; }
        public object? Errors { get; set; }

        public static ApiResponse<TData> Success(TData data, int statusCode = 200, string message = "Operation completed successfully")
        {
            return new ApiResponse<TData> { IsSuccess = true, StatusCode = statusCode, Message = message, Data = data };
        }

        public static ApiResponse<TData> Failure(int statusCode, string message, object? errors = null)
        {
            return new ApiResponse<TData> { IsSuccess = false, StatusCode = statusCode, Message = message, Errors = errors };
        }
    }
}
