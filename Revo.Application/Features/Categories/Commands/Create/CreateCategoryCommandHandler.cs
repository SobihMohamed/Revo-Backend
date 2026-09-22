using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Categories.Specifications;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.Categories.Commands.Create
{
    public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Guid>
    {
        private readonly IGenericRepo<Category> _categoryRepo;
        private readonly IUploadService _uploadService;
        public CreateCategoryCommandHandler(IGenericRepo<Category> genericRepo, IUploadService uploadService)
        {
            _categoryRepo = genericRepo;
            _uploadService = uploadService;
        }
        public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            // Check for Duplicate Name using Specification
            var spec = new CategoryByNameSpecification(request.NameAr, request.NameEn);
            var existingCategory = await _categoryRepo.FirstOrDefaultAsync(spec, cancellationToken);

            if (existingCategory != null)
            {
                return Result<Guid>.Failure(new Error("Category.DuplicateName", "The category name (in Arabic or English) is already registered."));
            }
            // 1 - handle image upload
            var ImageUploadResult = await _uploadService.UploadFileAsync(
                request.ImageUploadDto.Content,
                request.ImageUploadDto.FileName,
                cancellationToken
            );
            // 2 - check if upload was successful
            if (ImageUploadResult == null)
            {
                return Result<Guid>.Failure(new Error(
                    "Category.ImageUploadFailed",
                    "Failed to upload the category image to the server."));
            }
            // 3 - Create Category Entity
            var category = new Category
            {
                NameAr = request.NameAr,
                NameEn = request.NameEn,
                OrderIndex = request.OrderIndex,
                ImageUrl = ImageUploadResult.Url,
                ImagePublicId = ImageUploadResult.PublicId
            };

            // 4 - Save to Database
            await _categoryRepo.AddAsync(category, cancellationToken);

            // 5 - Return Success with the new ID
            return Result<Guid>.Success(category.Id);
        }
    }
}
