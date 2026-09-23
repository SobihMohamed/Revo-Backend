using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Requests.ContactRequest;
using Revo.API.Response.Commands;
using Revo.API.Resposes;
using Revo.Application.Common.Pagination;
using Revo.Application.Features.ContactRequests.Commands.Create;
using Revo.Application.Features.ContactRequests.Commands.MarkAsRead;
using Revo.Application.Features.ContactRequests.Dto;
using Revo.Application.Features.ContactRequests.Queries.GetAll;
using Revo.Application.Features.ContactRequests.Queries.GetById;
using Revo.Application.Features.ContactRequests.Queries.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;

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


        // [Authorize(Roles = "Admin")] 
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<ContactRequestDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] ContactRequestSpecParams specParams, CancellationToken cancellationToken)
        {
            var query = new GetAllContactRequestsQuery (specParams );
            var result = await Sender.Send(query, cancellationToken);

            return HandleResult(result);
        }

        // [Authorize(Roles = "Admin")] 
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ContactRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetContactRequestByIdQuery(id);
            var result = await Sender.Send(query, cancellationToken);

            return HandleResult(result);
        }

        // [Authorize(Roles = "Admin")] 
        [HttpPatch("{id:guid}/mark-as-read")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
        {
            var command = new MarkContactRequestAsReadCommand(id);
            var result = await Sender.Send(command, cancellationToken);

            return HandleResult(result);
        }
    }
}