using Revo.Application.Features.PortfolioItems.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.PortfolioItems.Queries.GetById
{
    public record GetPortfolioItemByIdQuery(Guid Id) : IQuery<PortfolioItemDetailsDto>;
}
