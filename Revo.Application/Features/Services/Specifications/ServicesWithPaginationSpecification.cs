using Ardalis.Specification;
using Revo.Application.Features.Services.Dto;
using Revo.Application.Features.Services.Queries.Helper; 
using Revo.Domain.Entities;

namespace Revo.Application.Features.Services.Specifications
{
    public class ServicesWithPaginationSpecification : Specification<Service, ServiceDto>
    {
        public ServicesWithPaginationSpecification(ServiceSpecParams specParams)
        {
            // 1. Filtering (Search)
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                Query.Where(x => x.NameAr.Contains(specParams.Search) ||
                                 x.NameEn.Contains(specParams.Search));
            }

            // 2. Sorting
            switch (specParams.Sort)
            {
                case ServiceSortOption.Newest:
                    Query.OrderByDescending(x => x.Id);
                    break;
                case ServiceSortOption.NameAsc:
                    Query.OrderBy(x => x.NameAr);
                    break;
                case ServiceSortOption.NameDesc:
                    Query.OrderByDescending(x => x.NameAr);
                    break;
                case ServiceSortOption.OrderAsc:
                default:
                    Query.OrderBy(x => x.OrderIndex);
                    break;
            }

            // 3. Pagination
            Query.Skip(specParams.PageSize * (specParams.PageIndex - 1))
                 .Take(specParams.PageSize);

            // 4. Mapping (Projection)
            Query.Select(s => new ServiceDto
            {
                Id = s.Id,
                NameAr = s.NameAr,
                NameEn = s.NameEn,
                DescriptionAr = s.DescriptionAr,
                DescriptionEn = s.DescriptionEn,
                ImageUrl = s.ImageUrl,
                OrderIndex = s.OrderIndex
            });
        }
    }
}