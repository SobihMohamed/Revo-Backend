using MediatR;
using Microsoft.AspNetCore.Mvc;
using Revo.API.Extention;
using Revo.API.Requests.Category;
using Revo.Application.Features.Categories.Commands.Create;
using System.Reflection;

namespace Revo.API.Controllers
{
    public class CategoriesController : BaseApiControllercs
    {
        public CategoriesController(ISender sender) : base(sender)
        {
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateCategoryCommand(
                request.NameAr,
                request.NameEn,
                request.OrderIndex,
                request.Image.ToUploadDto()
            );

            // 2. Dispatch
            var result = await Sender.Send(command, cancellationToken);

            // 3. Return
            return HandleResult(result);
        }
    }
}
