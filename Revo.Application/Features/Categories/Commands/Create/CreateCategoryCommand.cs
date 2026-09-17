using Revo.Application.Dto;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Commands.Create
{
    public record CreateCategoryCommand
    (
        string NameAr,
        string NameEn,
        int OrderIndex,
        ImageUploadDto ImageUploadDto

    ) : ICommand<Guid>;

}
