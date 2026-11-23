using Ardalis.Specification;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Categories.Specifications
{
    public class CategoryByNameSpecification : Specification<Category>
    {
        public CategoryByNameSpecification(string? nameAr, string nameEn)
        {
            if (string.IsNullOrWhiteSpace(nameAr))
            {
                Query.Where(c => c.NameEn == nameEn && !c.IsDeleted);
            }
            else
            {
                Query.Where(c => (c.NameAr == nameAr || c.NameEn == nameEn) && !c.IsDeleted);
            }
        }

        public CategoryByNameSpecification(string? nameAr, string nameEn, Guid excludeId)
        {
            if (string.IsNullOrWhiteSpace(nameAr))
            {
                Query.Where(c => c.NameEn == nameEn && c.Id != excludeId && !c.IsDeleted);
            }
            else
            {
                Query.Where(c => (c.NameAr == nameAr || c.NameEn == nameEn) && c.Id != excludeId && !c.IsDeleted);
            }
        }
    }
}