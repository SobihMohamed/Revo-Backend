using Ardalis.Specification;
using Revo.Application.Features.PortfolioItems.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.PortfolioItems.Specification
{
    public class PortfolioItemByIdSpec : Specification<Domain.Entities.PortfolioItem, PortfolioItemDetailsDto>
    {
        public PortfolioItemByIdSpec(Guid portfolioItemId)
        {
            Query.Where(p => p.Id == portfolioItemId);

            Query.Select(p => new PortfolioItemDetailsDto
            {
                Id = p.Id,
                CaptionAr = p.CaptionAr,
                CaptionEn = p.CaptionEn,
                OrderIndex = p.OrderIndex,
                CategoryId = p.CategoryId,
                CategoryNameAr = p.Category.NameAr,
                CategoryNameEn = p.Category.NameEn,

                MediaItems = p.MediaItems
                    .OrderBy(m => m.OrderIndex) 
                    .Select(m => new PortfolioMediaDto
                    {
                        Id = m.Id,
                        MediaUrl = m.MediaUrl,
                        CoverImageUrl = m.CoverImageUrl,
                        Type = m.Type,
                        OrderIndex = m.OrderIndex
                    }).ToList()
            });
        }
    }
}
