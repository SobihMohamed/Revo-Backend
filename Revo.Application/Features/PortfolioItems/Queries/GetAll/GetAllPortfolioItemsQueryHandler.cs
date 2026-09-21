using Revo.Application.Common.Pagination;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.PortfolioItems.Dto;
using Revo.Application.Features.PortfolioItems.Specification;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.PortfolioItems.Queries.GetAll
{
    public class GetAllPortfolioItemsQueryHandler
            : IQueryHandler<GetAllPortfolioItemsQuery, PaginationResponse<PortfolioItemListDto>>
    {
        private readonly IGenericRepo<PortfolioItem> _portfolioRepo;

        public GetAllPortfolioItemsQueryHandler(IGenericRepo<PortfolioItem> portfolioRepo)
        {
            _portfolioRepo = portfolioRepo;
        }

        public async Task<Result<PaginationResponse<PortfolioItemListDto>>> Handle(
            GetAllPortfolioItemsQuery request,
            CancellationToken cancellationToken)
        {
            var countSpec = new PortfolioItemsCountSpec(request.SpecParams);
            var totalCount = await _portfolioRepo.CountAsync(countSpec, cancellationToken);

            if (totalCount == 0)
            {
                var emptyResponse = new PaginationResponse<PortfolioItemListDto>(
                    request.SpecParams.PageIndex,
                    request.SpecParams.PageSize,
                    totalCount,
                    new List<PortfolioItemListDto>());

                return Result<PaginationResponse<PortfolioItemListDto>>.Success(emptyResponse);
            }

            var dataSpec = new PortfolioItemsWithCategoryAndMediaSpec(request.SpecParams);

            var data = await _portfolioRepo.ListAsync(dataSpec, cancellationToken);

            var paginationResponse = new PaginationResponse<PortfolioItemListDto>(
                request.SpecParams.PageIndex,
                request.SpecParams.PageSize,
                totalCount,
                data
            );

            return Result<PaginationResponse<PortfolioItemListDto>>.Success(paginationResponse);
        }
    }
}