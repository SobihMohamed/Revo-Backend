using MediatR;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Resposes;
using Revo.Domain.Shared;

namespace Revo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiControllercs : ControllerBase
    {
        protected readonly ISender Sender;

        public BaseApiControllercs(ISender sender)
        {
            Sender = sender;
        }

        // 1 - handle queries and created commands return data
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<T>.Success(result.Value!));
            }
            return ProcessFailure(result);
        }

        // 2 - handle queries and created commands return data with mapping to DTO
        protected IActionResult HandleResult<TIn, TOut>(Result<TIn> result, Func<TIn, TOut> mapper)
        {
            if (result.IsSuccess)
            {
                var responseData = mapper(result.Value!);
                return Ok(ApiResponse<TOut>.Success(responseData));
            }

            return ProcessFailure(result);
        }

        // 3 - handle queries and commands not returned data 
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<object>.Success(null!));
            }

            return ProcessFailure(result);
        }

        private IActionResult ProcessFailure(Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot handle failure for a successful result.");

            // (FluentValidation)
            if (result.Error is ValidationError validationError)
            {
                var validationResponse = ApiResponse<object>.Failure(
                    StatusCodes.Status400BadRequest,
                    "Validation Error",
                    validationError.ValidationErrors
                );
                return BadRequest(validationResponse);
            }

            var statusCode = result.Error.Code.Contains("NotFound", StringComparison.OrdinalIgnoreCase)
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status400BadRequest;

            var errorResponse = ApiResponse<object>.Failure(
                statusCode,
                result.Error.Code,
                result.Error.Message
            );

            return StatusCode(statusCode, errorResponse);
        }
    }
}