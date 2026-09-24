using Ardalis.Specification;
using Revo.Application.Features.Notifications.Dtos;
using Revo.Application.Features.Notifications.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Notifications.Specification
{
    public class GetAllNotificationByPaginationSpec : Specification<Domain.Entities.Notification, NotificationDto>
    {
        public GetAllNotificationByPaginationSpec(NotificationSpecParams specParams)
        {
            Query
                .Where(x => !specParams.IsRead.HasValue || x.IsRead == specParams.IsRead.Value)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize);

            Query.Select(n => new NotificationDto
            {
                Id = n.Id,
                TitleAr = n.TitleAr,
                TitleEn = n.TitleEn,
                MessageAr = n.MessageAr,
                MessageEn = n.MessageEn,
                TargetUrl = n.TargetUrl,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });
        }
    }
}
