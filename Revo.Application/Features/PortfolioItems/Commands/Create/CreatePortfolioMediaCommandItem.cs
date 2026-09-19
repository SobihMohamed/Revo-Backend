using Revo.Application.Dto;
using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.PortfolioItems.Commands.Create
{
    public record CreatePortfolioMediaCommandItem(
         MediaType Type,
         int OrderIndex,
         ImageUploadDto? File,
         string? VideoUrl,
         ImageUploadDto? CoverImage
     );
    public record CreatePortfolioItemCommand(
        string CaptionAr,
        string CaptionEn,
        int OrderIndex,
        Guid CategoryId,
        List<CreatePortfolioMediaCommandItem> MediaItems
    ) : ICommand<Guid>; 
}
