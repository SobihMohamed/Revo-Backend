using Ardalis.Specification;
using Revo.Application.Features.Services.Queries.Helper;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Services.Specifications
{
    public class ServicesCountSpecification : Specification<Service>
    {
        public ServicesCountSpecification(ServiceSpecParams specParams)
        {
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                Query.Where(x => x.NameAr.Contains(specParams.Search) ||
                                 x.NameEn.Contains(specParams.Search));
            }
        }
    }
}
