using MediatR;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Extention;
using Revo.API.Requests.Service;
using Revo.API.Response.Commands;
using Revo.API.Resposes;
using Revo.Application.Features.Services.Commands.Create;

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
                request.Image.ToUploadDto()
            );

            var result = await Sender.Send(command, cancellationToken);

            return HandleResult<Guid, ActionResponse>(result, id => new ActionResponse(id));
        }

    }
}
