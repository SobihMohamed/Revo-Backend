using Revo.Application.Abstraction.Caching;
using Revo.Application.Common.Pagination;
using Revo.Application.Features.Services.Dto;
using Revo.Application.Features.Services.Queries.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Services.Queries.GetAll
{
    public record GetAllServicesQuery(
      ServiceSpecParams SpecParams
  ) : IQuery<PaginationResponse<ServiceDto>>, ICacheableQuery
    {
        public string CacheGroup => "Services";

        public string CacheKey => !string.IsNullOrWhiteSpace(SpecParams.Search)
            ? null!
            : $"Sort_{SpecParams.Sort.ToString() ?? "Default"}" +
            $"_Page_{SpecParams.PageIndex}_Size_{SpecParams.PageSize}";

        public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
    }
}
