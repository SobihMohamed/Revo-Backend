using Ardalis.Specification;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Services.Specifications
{
    public class ServiceByNameSpecification : Specification<Service>
    {
        public ServiceByNameSpecification(string nameAr, string nameEn)
        {
            Query.Where(s => s.NameAr == nameAr || s.NameEn == nameEn);
        }

        public ServiceByNameSpecification(string nameAr, string nameEn, Guid excludeId)
        {
            Query.Where(s => (s.NameAr == nameAr || s.NameEn == nameEn) && s.Id != excludeId);
        }
    }
}
