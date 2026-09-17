using MediatR;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Extention;
using Revo.API.Requests;
using Revo.API.Requests.Category;
using Revo.API.Response.Commands;
using Revo.Application.Features.Categories.Commands.Create;
using Revo.Application.Features.Categories.Commands.Delete;
using Revo.Application.Features.Categories.Commands.Update;
using Revo.Application.Features.Categories.Queries.GetAll;  

namespace Revo.API.Controllers
{
    public class CategoriesController : BaseApiControllercs
    {
        public CategoriesController(ISender sender) : base(sender)
        {
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllCategoriesQuery(request.PageIndex, request.PageSize);

            var result = await Sender.Send(query, cancellationToken);

            return HandleResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateCategoryCommand(
                request.NameAr,
                request.NameEn,
                request.OrderIndex,
                request.Image.ToUploadDto()!
            );

            // 2. Dispatch
            var result = await Sender.Send(command, cancellationToken);

            // 3. Return
            return HandleResult<Guid, ActionResponse>(result ,id => new ActionResponse(id));
        }
        [HttpPut("{id:guid}")] 
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            // 1. Mapping Only (No Logic)
            var command = new UpdateCategoryCommand(
                id,
                request.NameAr,
                request.NameEn,
                request.OrderIndex,
                request.Image.ToUploadDto() 
            );

            // 2. Dispatch
            var result = await Sender.Send(command, cancellationToken);

            // 3. Return
            return HandleResult <Guid, ActionResponse>(result,id => new ActionResponse(id));
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand(id);

            var result = await Sender.Send(command, cancellationToken);

            return HandleResult(result);
        }
    }
}
