using Revo.Application.Features.Auth.Dto;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Contracts.Identity
{
    public interface IIdentityService
    {
        Task<Result> RegisterAdminAsync(Guid adminId, string email, string password, string fullName, CancellationToken cancellationToken);

        Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken);
    }
}