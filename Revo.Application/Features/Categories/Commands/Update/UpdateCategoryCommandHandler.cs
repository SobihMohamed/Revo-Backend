using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts.Repositories;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Commands.Update
{
    public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, Guid>
    {
        private readonly IGenericRepo<Category> _genericRepo;
        private readonly IUploadService _uploadService;
        public UpdateCategoryCommandHandler(IGenericRepo<Category> genericRepo , IUploadService uploadService) 
        {
            _genericRepo = genericRepo;
            _uploadService = uploadService;
        }
        public async Task<Result<Guid>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            // 1 - use specification to get the category by id
            var category = await _genericRepo.GetByIdAsync(request.Id,cancellationToken);
            if(category == null)
                return Result<Guid>.Failure(new Error("Category.NotFound","Category not found"));
            // 2 - update the category properties
            category.NameAr = request.NameAr;
            category.NameEn = request.NameEn;
            category.OrderIndex = request.OrderIndex;
            // 3- handle image upload if provided
            string? oldImagePublicId = null;

            if(request.ImageUploadDto != null)
            {
                // Upload the new image
                var uploadResult = await _uploadService.UploadFileAsync(request.ImageUploadDto.Content, request.ImageUploadDto.FileName, cancellationToken);
                if (uploadResult == null)
                    return Result<Guid>.Failure(new Error("ImageUpload.Failed", "Image upload failed"));
                // Store the old image public ID for deletion
                oldImagePublicId = category.ImagePublicId;
                // Update the category with the new image details
                category.ImageUrl = uploadResult.Url;
                category.ImagePublicId = uploadResult.PublicId;
            }
            // 4 - delete the old image if it exists
            await _genericRepo.UpdateAsync(category, cancellationToken);
            // 5 - save the updated category
            if (oldImagePublicId != null)
                await _uploadService.DeleteFileAsync(oldImagePublicId);
            return Result<Guid>.Success(category.Id);
        }
    }
}
