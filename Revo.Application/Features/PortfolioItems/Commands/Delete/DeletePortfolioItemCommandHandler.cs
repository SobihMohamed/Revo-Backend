using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.PortfolioItems.Commands.Specification;
using Revo.Domain.Entities;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Features.PortfolioItems.Commands.Delete
{
    public class DeletePortfolioItemCommandHandler : ICommandHandler<DeletePortfolioItemCommand, bool>
    {
        private readonly IGenericRepo<PortfolioItem> _portfolioRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUploadService _uploadService;
        public DeletePortfolioItemCommandHandler(
            IGenericRepo<PortfolioItem> portfolioRepo,
            IUnitOfWork unitOfWork,
            IUploadService uploadService)
        {
            _portfolioRepo = portfolioRepo;
            _unitOfWork = unitOfWork;
            _uploadService = uploadService;

        }
        public async Task<Result<bool>> Handle(DeletePortfolioItemCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetPortofolioWithMediaSpec(request.Id);
            var portfolioItem = await _portfolioRepo.FirstOrDefaultAsync(spec, cancellationToken);

            if (portfolioItem == null)
                return Result<bool>.Failure(new Error("PortfolioItemNotFound", "Portfolio item not found."));

            var publicIdsToDelete = new List<string>();
            foreach (var media in portfolioItem.MediaItems)
            {
                if (!string.IsNullOrEmpty(media.MediaPublicId))
                    publicIdsToDelete.Add(media.MediaPublicId);

                if (!string.IsNullOrEmpty(media.CoverImagePublicId))
                    publicIdsToDelete.Add(media.CoverImagePublicId);
            }

            await _portfolioRepo.DeleteAsync(portfolioItem);

            await _unitOfWork.SaveChanges(cancellationToken);

            if (publicIdsToDelete.Any())
                await _uploadService.DeleteFilesAsync(publicIdsToDelete);

            return Result<bool>.Success(true);
        }
    }
}
