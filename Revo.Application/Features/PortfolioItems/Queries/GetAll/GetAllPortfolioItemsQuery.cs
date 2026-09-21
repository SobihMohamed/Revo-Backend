using Revo.Application.Common.Pagination;
using Revo.Application.Features.PortfolioItems.Dto;
using Revo.Application.Features.PortfolioItems.Queries.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.PortfolioItems.Queries.GetAll
{
    public record GetAllPortfolioItemsQuery(PortfolioItemSpecParams SpecParams)
          : IQuery<PaginationResponse<PortfolioItemListDto>>;
}
