using Revo.Application.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Commands.Update
{
    public record UpdateCategoryCommand(
        Guid Id,
        string NameAr,
        string NameEn,
        int OrderIndex,
        ImageUploadDto? ImageUploadDto
        ) : ICommand<Guid>;

}
