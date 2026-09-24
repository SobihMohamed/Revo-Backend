using MediatR;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Resposes;
using Revo.Application.Features.Auth.Commands.Login;
using Revo.Application.Features.Auth.Dto;

namespace Revo.API.Controllers
{
    public class AuthController : BaseApiControllercs
    {
        public AuthController(ISender sender) : base(sender)
        {
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await Sender.Send(command, cancellationToken);
            return HandleResult(result);
        }
    }
}