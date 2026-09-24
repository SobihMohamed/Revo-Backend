using Revo.Application.Contracts.Identity;
using Revo.Application.Features.Auth.Dto;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
    {
        private readonly IIdentityService _identityService;

        public LoginCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.LoginAsync(request.Email, request.Password, cancellationToken);
        }
    }
}
