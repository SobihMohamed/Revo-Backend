using Revo.Application.Dto;
using Revo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.PortfolioItems.Commands.Update
{
   public record UpdatePortfolioMediaCommandItem(
        Guid? Id, 
        MediaType Type,
        int OrderIndex,
        string? VideoUrl,
        ImageUploadDto? File,
        ImageUploadDto? CoverImage
    );

    public record UpdatePortfolioItemCommand(
        Guid Id, 
        string CaptionAr,
        string CaptionEn, 
        int OrderIndex,  
        Guid CategoryId, 
        List<UpdatePortfolioMediaCommandItem> MediaItems 
    ) : ICommand<Guid>;
}
