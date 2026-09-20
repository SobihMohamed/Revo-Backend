using MediatR;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Extention;
using Revo.API.Requests.PortfolioItems;
using Revo.API.Response.Commands;
using Revo.API.Resposes;
using Revo.Application.Features.PortfolioItems.Commands.Create;

namespace Revo.API.Controllers
{
        public class PortfolioItemsController : BaseApiControllercs
        {
            public PortfolioItemsController(ISender sender) : base(sender)
            {
            }

            [HttpPost]
            [ProducesResponseType(typeof(ApiResponse<ActionResponse>), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> Create([FromForm] CreatePortfolioItemRequest request, CancellationToken cancellationToken)
            {
                // 1. Mapping from API Request to Application Command
                var commandMediaItems = request.MediaItems?.Select(m => new CreatePortfolioMediaCommandItem(
                    m.Type,
                    m.OrderIndex,
                    m.File.ToUploadDto(),       
                    m.VideoUrl,
                    m.CoverImage.ToUploadDto()    
                )).ToList() ?? new List<CreatePortfolioMediaCommandItem>();

                var command = new CreatePortfolioItemCommand(
                    request.CaptionAr,
                    request.CaptionEn,
                    request.OrderIndex,
                    request.CategoryId,
                    commandMediaItems
                );

                // 2. Dispatch to MediatR
                var result = await Sender.Send(command, cancellationToken);

                // 3. Handle Result using the Base Controller Logic
                return HandleResult<Guid, ActionResponse>(result, id => new ActionResponse(id));
            }
        }
    }