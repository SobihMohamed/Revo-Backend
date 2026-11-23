using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Categories.Specifications;
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
        private readonly IGenericRepo<Category> _categoryRepo;
        private readonly IUploadService _uploadService;

        public UpdateCategoryCommandHandler(IGenericRepo<Category> genericRepo, IUploadService uploadService)
        {
            _categoryRepo = genericRepo;
            _uploadService = uploadService;
        }

        public async Task<Result<Guid>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var normalizedNameAr = string.IsNullOrWhiteSpace(request.NameAr) ? null : request.NameAr.Trim();

            // Check Duplicate Name
            var spec = new CategoryByNameSpecification(normalizedNameAr, request.NameEn, request.Id);
            var duplicateCategory = await _categoryRepo.FirstOrDefaultAsync(spec, cancellationToken);

            if (duplicateCategory != null)
            {
                return Result<Guid>.Failure(new Error("Category.DuplicateName", "The category name (in Arabic or English) is already registered."));
            }

            // 1 - get the category by id
            var category = await _categoryRepo.GetByIdAsync(request.Id, cancellationToken);
            if (category == null)
                return Result<Guid>.Failure(new Error("Category.NotFound", "Category not found"));

            // 2 - update the category properties
            category.NameAr = normalizedNameAr;
            category.NameEn = request.NameEn;
            category.OrderIndex = request.OrderIndex;

            // 3 - handle image upload if provided
            string? oldImagePublicId = null;

            if (request.ImageUploadDto != null)
            {
                var uploadResult = await _uploadService.UploadFileAsync(request.ImageUploadDto.Content, request.ImageUploadDto.FileName, cancellationToken);
                if (uploadResult == null)
                    return Result<Guid>.Failure(new Error("Image.UploadFailed", "Image upload failed"));

                oldImagePublicId = category.ImagePublicId;
                category.ImageUrl = uploadResult.Url;
                category.ImagePublicId = uploadResult.PublicId;
            }

            // 4 - save the updated category
            await _categoryRepo.UpdateAsync(category, cancellationToken);

            // 5 - delete the old image if it exists
            if (oldImagePublicId != null)
                await _uploadService.DeleteFileAsync(oldImagePublicId);

            return Result<Guid>.Success(category.Id);
        }
    }
}