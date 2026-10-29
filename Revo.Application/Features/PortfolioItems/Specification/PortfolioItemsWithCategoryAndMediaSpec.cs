using Ardalis.Specification;
using Revo.Application.Features.PortfolioItems.Dto;
using Revo.Application.Features.PortfolioItems.Queries.Helper;
using Revo.Domain.Entities;
using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.PortfolioItems.Specification
{
    public class PortfolioItemsWithCategoryAndMediaSpec : Specification<PortfolioItem,PortfolioItemListDto>
    {
        public PortfolioItemsWithCategoryAndMediaSpec(PortfolioItemSpecParams specParams)
        {
            Query.Where(p =>
                (!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId) &&
                (string.IsNullOrEmpty(specParams.Search) ||
                 p.CaptionAr.ToLower().Contains(specParams.Search) ||
                 p.CaptionEn.ToLower().Contains(specParams.Search))
            );

            if (specParams.Sort.HasValue)
            {
                switch (specParams.Sort.Value)
                {
                    case PortfolioSortOption.OrderIndexAscending:
                        Query.OrderBy(p => p.OrderIndex);
                        break;
                    case PortfolioSortOption.OrderIndexDescending:
                        Query.OrderByDescending(p => p.OrderIndex);
                        break;
                    case PortfolioSortOption.NewestFirst:
                        Query.OrderByDescending(p => p.CreatedAt);
                        break;
                    case PortfolioSortOption.OldestFirst:
                        Query.OrderBy(p => p.CreatedAt);
                        break;
                }
            }
            else
            {
                Query.OrderBy(p => p.OrderIndex);
            }
            // Apply pagination
            if (specParams.PageIndex > 0 && specParams.PageSize > 0)
            {
                int skip = (specParams.PageIndex - 1) * specParams.PageSize;
                Query.Skip(skip).Take(specParams.PageSize);
            }
            else
            {
                Query.Skip(0).Take(10);
            }
            Query.Select(p => new PortfolioItemListDto
            {
                Id = p.Id,
                CaptionAr = p.CaptionAr,
                CaptionEn = p.CaptionEn,
                OrderIndex = p.OrderIndex,
                CategoryId = p.CategoryId,
                // make join automatically to get the category name
                CategoryNameAr = p.Category.NameAr,
                CategoryNameEn = p.Category.NameEn,
                // make join automatically to get the first media item as thumbnail
                ThumbnailUrl = p.MediaItems
                                .OrderBy(m => m.OrderIndex)
                                .Select(m => m.Type == MediaType.Video ? m.CoverImageUrl : m.MediaUrl)
                                .FirstOrDefault(),
                ThumbnailType = p.MediaItems
                                 .OrderBy(m => m.OrderIndex)
                                 .Select(m => (MediaType?)m.Type)
                                 .FirstOrDefault()
            });
        }
    }
}
