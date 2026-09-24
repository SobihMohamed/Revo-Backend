using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Auth.Dto
{
    public class TokenRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
