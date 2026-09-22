using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Services.Dto
{
    public class ServiceDto
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }
}
