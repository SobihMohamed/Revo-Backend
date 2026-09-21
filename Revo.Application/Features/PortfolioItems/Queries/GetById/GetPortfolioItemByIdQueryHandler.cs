using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.PortfolioItems.Dto;
using Revo.Application.Features.PortfolioItems.Specification;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.PortfolioItems.Queries.GetById
{
    public class GetPortfolioItemByIdQueryHandler
        : IQueryHandler<GetPortfolioItemByIdQuery, PortfolioItemDetailsDto>
    {
        private readonly IGenericRepo<PortfolioItem> _portfolioRepo;

        public GetPortfolioItemByIdQueryHandler(IGenericRepo<PortfolioItem> portfolioRepo)
        {
            _portfolioRepo = portfolioRepo;
        }

        public async Task<Result<PortfolioItemDetailsDto>> Handle(
            GetPortfolioItemByIdQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new PortfolioItemByIdSpec(request.Id);

            var item = await _portfolioRepo.FirstOrDefaultAsync(spec, cancellationToken);

            if (item == null)
            {
                return Result<PortfolioItemDetailsDto>.Failure(
                    new Error("PortfolioItemNotFound", "Portfolio item was not found."));
            }

            return Result<PortfolioItemDetailsDto>.Success(item);
        }
    }
}