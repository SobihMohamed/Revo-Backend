using Revo.Application.Common.Pagination;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Notifications.Dtos;
using Revo.Application.Features.Notifications.Specification;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Notifications.Queries.GetAll
{
    public class GetNotificationsQueryHandler : IQueryHandler<GetNotificationsQuery, PaginationResponse<NotificationDto>>
    {
        private readonly IGenericRepo<Notification> _notificationRepo;

        public GetNotificationsQueryHandler(IGenericRepo<Notification> notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<Result<PaginationResponse<NotificationDto>>> Handle(
            GetNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new GetAllNotificationByPaginationSpec(request.Params);
            var notifications = await _notificationRepo.ListAsync(spec, cancellationToken);

            var countSpec = new GetNotificationsCountSpec(request.Params);
            var totalCount = await _notificationRepo.CountAsync(countSpec, cancellationToken);

            var response = new PaginationResponse<NotificationDto>(
                request.Params.PageIndex,
                request.Params.PageSize,
                totalCount,
                notifications);

            return Result<PaginationResponse<NotificationDto>>.Success(response);
        }
    }
}