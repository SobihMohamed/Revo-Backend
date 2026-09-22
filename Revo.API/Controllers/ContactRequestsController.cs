using MediatR;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Requests.ContactRequest;
using Revo.API.Response.Commands;
using Revo.API.Resposes;
using Revo.Application.Features.ContactRequests.Commands;

namespace Revo.API.Controllers
{
    public class ContactRequestsController : BaseApiControllercs
    {
        public ContactRequestsController(ISender sender) : base(sender)
        {
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ActionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateContactRequestApiRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateContactRequestCommand(
                request.Name,
                request.PhoneNumber,
                request.Message,
                request.ServiceId
            );

            var result = await Sender.Send(command, cancellationToken);

            return HandleResult<Guid, ActionResponse>(result, id => new ActionResponse(id));
        }
    }
}
