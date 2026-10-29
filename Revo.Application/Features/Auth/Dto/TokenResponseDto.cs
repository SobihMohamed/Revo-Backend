using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Auth.Dto
{
    public class TokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireOn { get; set; }
    }
}
