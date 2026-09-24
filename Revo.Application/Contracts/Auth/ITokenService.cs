using Revo.Application.Features.Auth.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Contracts.Auth
{
    public interface ITokenService
    {
        Task<TokenResponseDto> CreateTokenAsync(TokenRequestDto request);
    }
}
