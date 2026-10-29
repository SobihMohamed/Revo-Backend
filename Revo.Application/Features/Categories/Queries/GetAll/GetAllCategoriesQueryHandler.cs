using Revo.Application.Common.Pagination;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Categories.Dto;
using Revo.Application.Features.Categories.Specifications;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System.Threading;
using System.Threading.Tasks;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Queries.GetAll
{
    public class GetAllCategoriesQueryHandler : IQueryHandler<GetAllCategoriesQuery, PaginationResponse<CategoryDto>>
    {
        private readonly IGenericRepo<Category> _genericRepo;

        public GetAllCategoriesQueryHandler(IGenericRepo<Category> genericRepo)
        {
            _genericRepo = genericRepo;
        }

        public async Task<Result<PaginationResponse<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var dataSpec = new ActiveCategoriesSpec(request.PageIndex, request.PageSize);
            var countSpec = new ActiveCategoriesCountSpec();

            var data = await _genericRepo.ListAsync(dataSpec, cancellationToken);
            var count = await _genericRepo.CountAsync(countSpec, cancellationToken);

            var paginationResponse = new PaginationResponse<CategoryDto>(
                request.PageIndex,
                request.PageSize,
                count,
                data);

            return Result<PaginationResponse<CategoryDto>>.Success(paginationResponse);
        }
    }
}