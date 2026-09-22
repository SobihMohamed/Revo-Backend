using MediatR;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Extention;
using Revo.API.Requests.Service;
using Revo.API.Response.Commands;
using Revo.API.Resposes;
using Revo.Application.Features.Services.Commands.Create;
using Revo.Application.Features.Services.Commands.Update;

namespace Revo.API.Controllers
{
    public class ServicesController : BaseApiControllercs
    {
        public ServicesController(ISender sender) : base(sender)
        {
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ActionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] CreateServiceRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateServiceCommand(
                request.NameAr,
                request.NameEn,
                request.DescriptionAr,
                request.DescriptionEn,
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
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromForm] UpdateServiceRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Mapping from API Request to Command
            var command = new UpdateServiceCommand(
                id,
                request.NameAr,
                request.NameEn,
                request.DescriptionAr,
                request.DescriptionEn,
                request.OrderIndex,
                request.Image?.ToUploadDto() 
            );

            // 2. Dispatch to MediatR
            var result = await Sender.Send(command, cancellationToken);

            // 3. Handle Result
            return HandleResult<Guid, ActionResponse>(result, updatedId => new ActionResponse(updatedId));
        }

    }
}
