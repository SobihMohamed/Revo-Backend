using Revo.Application.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Services.Commands.Update
{
    public record UpdateServiceCommand
        (
            Guid Id,
            string NameAr,
            string NameEn,
            string DescriptionAr,
            string DescriptionEn,
            int OrderIndex,
            ImageUploadDto? UploadDto = null
        ):ICommand<Guid>;
}
