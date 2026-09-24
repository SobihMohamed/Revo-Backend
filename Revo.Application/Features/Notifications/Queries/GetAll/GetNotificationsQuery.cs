using Revo.Application.Common.Pagination;
using Revo.Application.Features.Notifications.Dtos;
using Revo.Application.Features.Notifications.Helper;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Notifications.Queries.GetAll
{
    public record GetNotificationsQuery(NotificationSpecParams Params)
          : IQuery<PaginationResponse<NotificationDto>>;
}
