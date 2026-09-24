using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Revo.Application.Contracts.Auth;
using Revo.Application.Features.Auth.Dto;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Revo.Infrastructure.ServicesImplementation
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<TokenResponseDto> CreateTokenAsync(TokenRequestDto request)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, request.UserId),
                new Claim(ClaimTypes.Name, request.UserName),
                new Claim(ClaimTypes.Email, request.Email)
            };

            foreach (var role in request.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = _configuration["JwtTokenSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expireTime = DateTime.UtcNow.AddHours(4);

            var tokenDesc = new JwtSecurityToken(
                issuer: _configuration["JwtTokenSettings:Issuer"],
                audience: _configuration["JwtTokenSettings:Audience"],
                claims: claims,
                expires: expireTime,
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDesc);

            return Task.FromResult(new TokenResponseDto
            {
                Token = token,
                ExpireOn = expireTime
            });
        }
    }
}