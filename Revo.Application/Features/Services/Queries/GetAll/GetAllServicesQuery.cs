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
    ) : IQuery<PaginationResponse<ServiceDto>>;
}
