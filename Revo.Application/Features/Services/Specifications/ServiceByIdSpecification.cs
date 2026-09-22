using Ardalis.Specification;
using Revo.Application.Features.Services.Dto;
using Revo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Services.Specifications
{
    public class ServiceByIdSpecification : Specification<Service, ServiceDto>
    {
        public ServiceByIdSpecification(Guid id)
        {
            Query.Where(s => s.Id == id);

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