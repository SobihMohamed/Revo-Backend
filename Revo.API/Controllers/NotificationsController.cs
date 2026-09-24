using MediatR;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Resposes;
using Revo.Application.Common.Pagination;
using Revo.Application.Features.Notifications.Dtos;
using Revo.Application.Features.Notifications.Helper;
using Revo.Application.Features.Notifications.Queries.GetAll;
using Revo.Application.Features.Notifications.Queries.GetUnread;

namespace Revo.API.Controllers
{
    public class NotificationsController : BaseApiControllercs
    {
        public NotificationsController(ISender sender) : base(sender)
        {
        }

        // [Authorize(Roles = "Admin")] 
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponse<NotificationDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] NotificationSpecParams specParams, CancellationToken cancellationToken)
        {
            var query = new GetNotificationsQuery(specParams);
            var result = await Sender.Send(query, cancellationToken);

            return HandleResult(result);
        }

        // [Authorize(Roles = "Admin")] 
        [HttpGet("unread-count")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
        {
            var query = new GetUnreadNotificationsCountQuery();
            var result = await Sender.Send(query, cancellationToken);

            return HandleResult(result);
        }
    }
}