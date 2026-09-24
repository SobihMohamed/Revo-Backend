using Microsoft.AspNetCore.Identity;
using Revo.Application.Contracts.Auth;
using Revo.Application.Contracts.Identity;
using Revo.Application.Features.Auth.Dto;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public IdentityService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<AuthResponse>.Failure(new Error("Auth.Failed", "The email address is not found"));

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                return Result<AuthResponse>.Failure(new Error("Auth.Failed", "The email address or password is incorrect"));

            var roles = await _userManager.GetRolesAsync(user);

            var tokenRequest = new TokenRequestDto
            {
                UserId = user.Id.ToString(),
                UserName = user.FullName ?? user.UserName, 
                Email = user.Email!,
                Roles = roles
            };

            var tokenResponse = await _tokenService.CreateTokenAsync(tokenRequest);

            var response = new AuthResponse
            {
                Token = tokenResponse.Token,
                IsAuthenticated = true,
                ExpireOn = tokenResponse.ExpireOn,
                Email = user.Email!,
                Name = user.FullName ?? user.UserName, 
                Roles = roles
            };

            return Result<AuthResponse>.Success(response);
        }

        public async Task<Result> RegisterAdminAsync(Guid adminId, string email, string password, string fullName, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                Id = adminId, 
                UserName = email,
                Email = email,
                FullName = fullName, 
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return Result.Failure(new Error("Auth.RegisterFailed", "The registration of the admin user failed"));

            await _userManager.AddToRoleAsync(user, "Admin");

            return Result.Success();
        }
    }
}