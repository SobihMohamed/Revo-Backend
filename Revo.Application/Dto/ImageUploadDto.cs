using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Dto
{
    public record ImageUploadDto(Stream Content, string FileName);
}
