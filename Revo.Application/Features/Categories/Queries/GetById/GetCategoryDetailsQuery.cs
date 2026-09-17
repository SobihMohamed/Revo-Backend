using Revo.Application.Features.Categories.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Queries.GetById
{
    public record GetCategoryDetailsQuery(Guid Id, int PageIndex = 1, int PageSize = 10)
            : IQuery<CategoryDetailsDto>;
}
