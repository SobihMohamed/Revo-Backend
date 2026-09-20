using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.PortfolioItems.Commands.Specification;
using Revo.Domain.Entities;
using Revo.Domain.Enums;
using Revo.Domain.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.PortfolioItems.Commands.Update
{
    public class UpdatePortfolioItemCommandHandler : ICommandHandler<UpdatePortfolioItemCommand, Guid>
    {
        private readonly IGenericRepo<PortfolioItem> _PortofolioRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUploadService _uploadService;
        public UpdatePortfolioItemCommandHandler(IGenericRepo<PortfolioItem> portofolioRepo, IUploadService uploadService , IUnitOfWork unitOfWork)
        {
            _PortofolioRepo = portofolioRepo;
            _uploadService = uploadService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(UpdatePortfolioItemCommand request, CancellationToken cancellationToken)
        {
            // get portofolio with media item
            var spec = new GetPortofolioWithMediaSpec(request.Id);
            var portfolioItem = await _PortofolioRepo.FirstOrDefaultAsync(spec, cancellationToken);
            if (portfolioItem == null)
                return Result<Guid>.Failure(new Error("PortfolioItemNotFound", $"Portfolio item with id {request.Id} not found."));
            // declare 2 variable 
            var publicIdsToDeleteAfterSaveInDb = new List<string>(); // to delete the media items from cloudinary after save in db
            var publicIdsToRollbackIfErrorInDb = new ConcurrentBag<string>(); // to delete the media items from cloudinary if error occurs

            try
            {
                // handle deletions 
                HandleDeletions(portfolioItem, request.MediaItems, publicIdsToDeleteAfterSaveInDb);
                // handle updates and uploads 
                var isUploadSuccess = await HandleUploadsAndUpdatesAsync(
                    portfolioItem, request.MediaItems, publicIdsToRollbackIfErrorInDb, cancellationToken);
                // if upload failed, cleanup cloudinary and return error
                if (!isUploadSuccess)
                {
                    await CleanupCloudinaryAsync(publicIdsToRollbackIfErrorInDb);
                    return Result<Guid>.Failure(new Error("MediaUploadFailed", "Failed to upload one or more new media items."));
                }
                UpdateBasicDetails(portfolioItem, request);
                await _PortofolioRepo.UpdateAsync(portfolioItem, cancellationToken);
                await _unitOfWork.SaveChanges(cancellationToken);
                // cleanup cloudinary for deleted media items after save in db
                await CleanupCloudinaryAsync(publicIdsToDeleteAfterSaveInDb);

                return Result<Guid>.Success(portfolioItem.Id);
            }
            catch (Exception ex)
            {
                // if any error occurs, cleanup cloudinary for newly uploaded media items
                await CleanupCloudinaryAsync(publicIdsToRollbackIfErrorInDb);
                return Result<Guid>.Failure(new Error("System.Error", "An unexpected error occurred."));
            }
        }
        private void UpdateBasicDetails(PortfolioItem portfolioItem, UpdatePortfolioItemCommand request)
        {
            portfolioItem.CaptionAr = request.CaptionAr;
            portfolioItem.CaptionEn = request.CaptionEn;
            portfolioItem.OrderIndex = request.OrderIndex;
            portfolioItem.CategoryId = request.CategoryId;
        }
        private async Task<bool> HandleUploadsAndUpdatesAsync(PortfolioItem portfolioItem,  List<UpdatePortfolioMediaCommandItem> updatedMediaItems, ConcurrentBag<string> publicIdsToRollbackIfErrorInDb, CancellationToken cancellationToken)
        {
            var tasks = updatedMediaItems.Select(async updateMedia =>
            {
                if(updateMedia.Id == null)
                {
                    var newMedia = await ProcessNewMediaAsync(updateMedia, publicIdsToRollbackIfErrorInDb, cancellationToken);
                    if (newMedia == null)
                        return false;
                    portfolioItem.MediaItems.Add(newMedia);
                    return true;
                }
                var existingMedia = portfolioItem.MediaItems.FirstOrDefault(m => m.Id == updateMedia.Id);
                if(existingMedia != null)
                    existingMedia.OrderIndex = updateMedia.OrderIndex;
                return true;
            });
            var results = await Task.WhenAll(tasks);
            return results.All(success => success == true);
        }
        private async Task<PortfolioMedia?> ProcessNewMediaAsync(UpdatePortfolioMediaCommandItem updateMedia, ConcurrentBag<string> publicIdsToRollbackIfErrorInDb, CancellationToken cancellationToken)
        {
            var mediaEntity = new PortfolioMedia
            {
                Type = updateMedia.Type,
                OrderIndex = updateMedia.OrderIndex
            };
            switch(mediaEntity.Type)
            {
                case MediaType.Image:
                    if (updateMedia.File == null)
                        return null;
                    var imageUploadResult = await _uploadService.UploadFileAsync(updateMedia.File.Content, updateMedia.File.FileName,cancellationToken);
                    if (imageUploadResult == null)
                        return null;
                    publicIdsToRollbackIfErrorInDb.Add(imageUploadResult.PublicId);
                    mediaEntity.MediaUrl = imageUploadResult.Url;
                    mediaEntity.MediaPublicId = imageUploadResult.PublicId;

                    break;
                case MediaType.Video:
                    if (string.IsNullOrEmpty(updateMedia.VideoUrl))
                        return null;
                    mediaEntity.MediaUrl = updateMedia.VideoUrl;
                    if(updateMedia.CoverImage != null)
                    {
                        var coverImageUploadResult = await _uploadService.UploadFileAsync(updateMedia.CoverImage.Content, updateMedia.CoverImage.FileName, cancellationToken);
                        if (coverImageUploadResult == null)
                            return null;
                        publicIdsToRollbackIfErrorInDb.Add(coverImageUploadResult.PublicId);
                        mediaEntity.CoverImageUrl = coverImageUploadResult.Url;
                        mediaEntity.CoverImagePublicId = coverImageUploadResult.PublicId;
                    }
                    break;
                default:
                    return null;
            }
            return mediaEntity;
        }
        private void HandleDeletions(PortfolioItem portfolioItem, List<UpdatePortfolioMediaCommandItem> updatedMediaItems, List<string> publicIdsToDeleteAfterSaveInDb)
        {
            // Find media items that are in the existing portfolio item but not in the updated list
            var mediaItemsToDelete = portfolioItem.MediaItems
                .Where(existingMedia => !updatedMediaItems.Any(updatedMedia => updatedMedia.Id == existingMedia.Id)) // keep only the media items that are not in the updated list
                .ToList();
            foreach (var mediaItem in mediaItemsToDelete)
            {
                if (!string.IsNullOrEmpty(mediaItem.MediaPublicId))
                    publicIdsToDeleteAfterSaveInDb.Add(mediaItem.MediaPublicId);
                if (!string.IsNullOrEmpty(mediaItem.CoverImagePublicId))
                    publicIdsToDeleteAfterSaveInDb.Add(mediaItem.CoverImagePublicId);
                portfolioItem.MediaItems.Remove(mediaItem);
            }
        }
        private async Task CleanupCloudinaryAsync(IEnumerable<string> publicIds)
        {
            await _uploadService.DeleteFilesAsync(publicIds);
        }
    }
}
