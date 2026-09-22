using Revo.Application.Features.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Services.Queries.GetById
{
    public record GetServiceByIdQuery(Guid Id) : IQuery<ServiceDto>;
}
