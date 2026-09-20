using MediatR;
using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Domain.Entities;
using Revo.Domain.Enums;
using Revo.Domain.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.PortfolioItems.Commands.Create
{
    public class CreatePortfolioItemCommandHandler : ICommandHandler<CreatePortfolioItemCommand, Guid>
    {
        private readonly IGenericRepo<Category> _categoryRepo;
        private readonly IGenericRepo<PortfolioItem> _portfolioItemRepo;
        private readonly IUploadService _uploadService;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePortfolioItemCommandHandler(
            IGenericRepo<Category> categoryRepo,
            IGenericRepo<PortfolioItem> portfolioItemRepo,
            IUploadService uploadService,
            IUnitOfWork unitOfWork)
        {
            _categoryRepo = categoryRepo;
            _portfolioItemRepo = portfolioItemRepo;
            _uploadService = uploadService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreatePortfolioItemCommand request, CancellationToken cancellationToken)
        {
            // 1 - Validate the category exists
            var category = await _categoryRepo.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null || category.IsDeleted)
                return Result<Guid>.Failure(new Error("CategoryNotFound", $"Category with ID {request.CategoryId} not found."));

            // 2 - Create the storage IDs for the media items uploaded to the cloud storage
            // to remove them if the DB transaction fails
            var uploadsPublicIds = new ConcurrentBag<string>();
            try 
            {
                // 3 - Upload the media items to the cloud storage
                var uploadsTasks = request.MediaItems
                 .Select((mediaItem, index) =>
                 {
                     if (mediaItem.OrderIndex == 0)
                         mediaItem = mediaItem with { OrderIndex = index + 1 };

                     return UploadMediaItemsAsync(mediaItem, uploadsPublicIds, cancellationToken);
                 })
                 .ToList();
                var processedMediaArray = await Task.WhenAll(uploadsTasks);
                if(processedMediaArray.Any(media => media == null))
                {
                    await RollbackUploadedFilesAsync(uploadsPublicIds);
                    return Result<Guid>.Failure(new Error("MediaUploadFailed", "One or more media items failed to upload."));
                }
                var protofolioItem = new PortfolioItem
                {
                    CaptionAr = request.CaptionAr,
                    CaptionEn = request.CaptionEn,
                    OrderIndex = request.OrderIndex,
                    CategoryId = request.CategoryId,
                    MediaItems = processedMediaArray.Where(media => media != null).ToList()!
                };
                await _portfolioItemRepo.AddAsync(protofolioItem, cancellationToken);
                await _unitOfWork.SaveChanges(cancellationToken);
                return Result<Guid>.Success(protofolioItem.Id);
            }
            catch(Exception ex)
            {
                await RollbackUploadedFilesAsync(uploadsPublicIds);
                return Result<Guid>.Failure(new Error("System.Error", "An unexpected error occurred while saving the item."));
            }
        }
        private async Task<PortfolioMedia?> UploadMediaItemsAsync(CreatePortfolioMediaCommandItem mediaItems, ConcurrentBag<string> uploadsPublicIds, CancellationToken cancellationToken)
        {
            var mediaEntity = new PortfolioMedia
            {
                Type = mediaItems.Type,
                OrderIndex = mediaItems.OrderIndex
            };

            switch (mediaItems.Type)
            {
                case MediaType.Image:
                    if (mediaItems.File == null) return null;

                    var uploadResult = await _uploadService.UploadFileAsync(mediaItems.File.Content, mediaItems.File.FileName, cancellationToken);
                    if (uploadResult == null) return null;

                    uploadsPublicIds.Add(uploadResult.PublicId);
                    mediaEntity.MediaUrl = uploadResult.Url;
                    mediaEntity.MediaPublicId = uploadResult.PublicId;
                    break;

                case MediaType.Video:
                    if (string.IsNullOrEmpty(mediaItems.VideoUrl)) return null;

                    mediaEntity.MediaUrl = mediaItems.VideoUrl;

                    if (mediaItems.CoverImage != null)
                    {
                        var coverUploadResult = await _uploadService.UploadFileAsync(mediaItems.CoverImage.Content, mediaItems.CoverImage.FileName, cancellationToken);
                        if (coverUploadResult == null) return null;

                        uploadsPublicIds.Add(coverUploadResult.PublicId);
                        mediaEntity.CoverImageUrl = coverUploadResult.Url;
                        mediaEntity.CoverImagePublicId = coverUploadResult.PublicId;
                    }
                    break;

                default:
                    return null;
            }

            return mediaEntity;
        }

        private async Task RollbackUploadedFilesAsync(ConcurrentBag<string> publicIds)
        {
            if (publicIds.Any())
            {
                await _uploadService.DeleteFilesAsync(publicIds);
            }
        }
    }
}
