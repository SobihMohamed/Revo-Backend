using Ardalis.Specification;
using Revo.Application.Features.Categories.Dto;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Specifications
{
    public class PortfolioItemsSnippetSpec : Specification<PortfolioItem, PortfolioItemSnippetDto>
    {
        public PortfolioItemsSnippetSpec(Guid categoryId, int pageIndex, int pageSize)
        {
            Query
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.OrderIndex)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PortfolioItemSnippetDto(
                    p.Id,
                    p.CaptionAr,
                    p.CaptionEn,
                    p.OrderIndex,
                    p.MediaItems.FirstOrDefault() != null ? p.MediaItems.FirstOrDefault()!.MediaUrl : null
                ));
        }
    }
}
