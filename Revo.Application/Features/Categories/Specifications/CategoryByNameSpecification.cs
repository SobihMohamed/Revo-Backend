using Ardalis.Specification;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Categories.Specifications
{
    public class CategoryByNameSpecification : Specification<Category>
    {
        public CategoryByNameSpecification(string nameAr, string nameEn)
        {
            Query.Where(c => c.NameAr == nameAr || c.NameEn == nameEn);
        }
        public CategoryByNameSpecification(string nameAr, string nameEn, Guid excludeId)
        {
            Query.Where(c => (c.NameAr == nameAr || c.NameEn == nameEn) && c.Id != excludeId);
        }
    }
}
