using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Features.Auth.Dto
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public DateTime ExpireOn { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
