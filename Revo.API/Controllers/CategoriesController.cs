using MediatR;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Extention;
using Revo.API.Requests;
using Revo.API.Requests.Category;
using Revo.API.Response.Commands;
using Revo.API.Resposes;
using Revo.Application.Common.Pagination;
using Revo.Application.Features.Categories.Commands.Create;
using Revo.Application.Features.Categories.Commands.Delete;
using Revo.Application.Features.Categories.Commands.Update;
using Revo.Application.Features.Categories.Dto;
using Revo.Application.Features.Categories.Queries.GetAll;
using Revo.Application.Features.Categories.Queries.GetById;

namespace Revo.API.Controllers
{
    public class CategoriesController : BaseApiControllercs
    {
        public CategoriesController(ISender sender) : base(sender)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<CategoryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllCategoriesQuery(request.PageIndex, request.PageSize);
            var result = await Sender.Send(query, cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, [FromQuery] PaginationRequest request, CancellationToken cancellationToken)
        {
            var query = new GetCategoryDetailsQuery(id, request.PageIndex, request.PageSize);
            var result = await Sender.Send(query, cancellationToken);
            return HandleResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ActionResponse>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)] 
        public async Task<IActionResult> Create([FromForm] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateCategoryCommand(
                request.NameAr,
                request.NameEn,
                request.OrderIndex,
                request.Image.ToUploadDto()!
            );
            var result = await Sender.Send(command, cancellationToken);
            return HandleResult<Guid, ActionResponse>(result, id => new ActionResponse(id));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ActionResponse>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateCategoryCommand(
                id,
                request.NameAr,
                request.NameEn,
                request.OrderIndex,
                request.Image.ToUploadDto()
            );
            var result = await Sender.Send(command, cancellationToken);
            return HandleResult<Guid, ActionResponse>(result, id => new ActionResponse(id));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand(id);
            var result = await Sender.Send(command, cancellationToken);
            return HandleResult(result);
        }
    }
}