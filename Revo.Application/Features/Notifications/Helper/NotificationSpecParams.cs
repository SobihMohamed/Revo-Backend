using Revo.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Notifications.Helper
{
    public record NotificationSpecParams : PaginationParams
    {
        public bool? IsRead { get; init; }
    }
}
