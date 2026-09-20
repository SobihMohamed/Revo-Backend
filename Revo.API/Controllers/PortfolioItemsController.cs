using MediatR;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Extention;
using Revo.API.Requests.PortfolioItems.Create;
using Revo.API.Requests.PortfolioItems.Update;
using Revo.API.Response.Commands;
using Revo.API.Resposes;
using Revo.Application.Features.PortfolioItems.Commands.Create;
using Revo.Application.Features.PortfolioItems.Commands.Delete;
using Revo.Application.Features.PortfolioItems.Commands.Update;

namespace Revo.API.Controllers
{
    public class PortfolioItemsController : BaseApiControllercs
    {
        public PortfolioItemsController(ISender sender) : base(sender)
        {
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ActionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Create([FromForm] CreatePortfolioItemRequest request, CancellationToken cancellationToken)
        {
           // 1. Mapping from API Request to Application Command
           var commandMediaItems = request.MediaItems?.Select(m => new CreatePortfolioMediaCommandItem(
               m.Type,
               m.OrderIndex,
               m.File.ToUploadDto(),       
               m.VideoUrl,
               m.CoverImage.ToUploadDto()    
           )).ToList() ?? new List<CreatePortfolioMediaCommandItem>();

           var command = new CreatePortfolioItemCommand(
               request.CaptionAr,
               request.CaptionEn,
               request.OrderIndex,
               request.CategoryId,
               commandMediaItems
           );
            // 2. Dispatch to MediatR
            var result = await Sender.Send(command, cancellationToken);
            // 3. Handle Result using the Base Controller Logic
            return HandleResult<Guid, ActionResponse>(result, id => new ActionResponse(id));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ActionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] UpdatePortfolioItemRequest request, CancellationToken cancellationToken)
        {
            // 1. Mapping from API Request to Application Command
            var commandMediaItems = request.MediaItems?.Select(m => new UpdatePortfolioMediaCommandItem(
                m.Id, 
                m.Type,
                m.OrderIndex,
                m.VideoUrl,
                m.File.ToUploadDto(),
                m.CoverImage.ToUploadDto()
            )).ToList() ?? new List<UpdatePortfolioMediaCommandItem>();

            var command = new UpdatePortfolioItemCommand(
                id, 
                request.CaptionAr,
                request.CaptionEn,
                request.OrderIndex,
                request.CategoryId,
                commandMediaItems
            );

            // 2. Dispatch to MediatR
            var result = await Sender.Send(command, cancellationToken);

            // 3. Handle Result using the Base Controller Logic
            return HandleResult<Guid, ActionResponse>(result, updatedId => new ActionResponse(updatedId));
        }
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeletePortfolioItemCommand(id);

            var result = await Sender.Send(command, cancellationToken);

            return HandleResult(result);
        }
    }
}