using Revo.Application.Common.Pagination;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Categories.Dto;
using Revo.Application.Features.Categories.Specifications;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Queries.GetById
{
    public class GetCategoryDetailsQueryHandler : IQueryHandler<GetCategoryDetailsQuery, CategoryDetailsDto>
    {
        private readonly IGenericRepo<Category> _categoryRepo;
        private readonly IGenericRepo<PortfolioItem> _portfolioItemRepo;

        public GetCategoryDetailsQueryHandler(
            IGenericRepo<Category> categoryRepo,
            IGenericRepo<PortfolioItem> portfolioItemRepo)
        {
            _categoryRepo = categoryRepo;
            _portfolioItemRepo = portfolioItemRepo;
        }

        public async Task<Result<CategoryDetailsDto>> Handle(GetCategoryDetailsQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepo.GetByIdAsync(request.Id);
            if (category == null || category.IsDeleted)
                return Result<CategoryDetailsDto>.Failure(new Error("Category.NotFound", "The category does not exist or has been deleted."));

            var countSpec = new PortfolioItemsCountByCategoryIdSpec(request.Id);
            var totalItemsCount = await _portfolioItemRepo.CountAsync(countSpec, cancellationToken);

            var itemsSpec = new PortfolioItemsSnippetSpec(request.Id, request.PageIndex, request.PageSize);
            var paginatedItems = await _portfolioItemRepo.ListAsync(itemsSpec, cancellationToken);

            var itemsPaginationResponse = new PaginationResponse<PortfolioItemSnippetDto>(
                request.PageIndex,
                request.PageSize,
                totalItemsCount,
                paginatedItems
            );

            var categoryDetailsDto = new CategoryDetailsDto(
                category.Id,
                category.NameAr,
                category.NameEn,
                category.ImageUrl,
                category.OrderIndex,
                itemsPaginationResponse
            );

            return Result<CategoryDetailsDto>.Success(categoryDetailsDto);
        }
    }
}