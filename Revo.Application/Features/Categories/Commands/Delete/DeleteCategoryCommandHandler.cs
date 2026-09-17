using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Categories.Commands.Delete;
using Revo.Application.Features.Categories.Specifications; // 👈 مسار الـ Spec
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Commands.Delete
{
    public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, bool>
    {
        private readonly IGenericRepo<Category> _genericRepo;
        private readonly IUploadService _uploadService;

        public DeleteCategoryCommandHandler(IGenericRepo<Category> genericRepo, IUploadService uploadService)
        {
            _genericRepo = genericRepo;
            _uploadService = uploadService;
        }

        public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            // 1- Use Specification to get Category WITH its PortfolioItems
            var spec = new CategoryWithPortfolioItemsSpec(request.Id);
            var category = await _genericRepo.FirstOrDefaultAsync(spec, cancellationToken);

            if (category == null)
                return Result<bool>.Failure(new Error("Category.NotFound", "The category does not exist."));

            // 2- Business Validation: Restrict deletion if it has items
            if (category.PortfolioItems != null && category.PortfolioItems.Count > 0)
                return Result<bool>.Failure(new Error("Category.HasPortfolioItems", "Cannot delete this category because it contains active portfolio items."));

            var imagePublicId = category.ImagePublicId;

            // 3- SOFT DELETE in Database FIRST
            category.IsDeleted = true; 
            await _genericRepo.UpdateAsync(category, cancellationToken);

            // 4- CLEANUP: Delete the image from Cloudinary AFTER DB saves successfully
            if (!string.IsNullOrEmpty(imagePublicId))
                await _uploadService.DeleteFileAsync(imagePublicId);

            // 5- Return success
            return Result<bool>.Success(true);
        }
    }
}