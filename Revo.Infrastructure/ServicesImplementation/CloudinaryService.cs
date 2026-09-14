using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Revo.Application.Abstraction.Services;
using Revo.Infrastructure.Settings;

namespace Revo.Infrastructure.ServicesImplementation
{
    public class CloudinaryService : IUploadService
    {
        private readonly ILogger<CloudinaryService> _logger;
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(ILogger<CloudinaryService> logger, IOptions<CloudinarySettings> cloudinarySettings)
        {
            _logger = logger;
            var account = new Account(cloudinarySettings.Value.CloudName, cloudinarySettings.Value.ApiKey, cloudinarySettings.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }
        public async Task<UploadReturnedDto?> UploadFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = "RevoAssets",
                    UseFilename = true,
                    UniqueFilename = true
                };

                var result = await _cloudinary.UploadAsync(uploadParams , cancellationToken);

                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary upload error: {ErrorMessage}", result.Error.Message);
                    return null;
                }

                return new UploadReturnedDto(result.SecureUrl.ToString(), result.PublicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while uploading to Cloudinary.");
                return null;
            }
        }

        public async Task<IEnumerable<UploadReturnedDto>> UploadFilesAsync(IEnumerable<(Stream fileStream, string fileName)> files, CancellationToken cancellationToken = default)
        {
            var tasks = files.Select(file => UploadFileAsync(file.fileStream, file.fileName, cancellationToken));

            var result = await Task.WhenAll(tasks);

            return result.Where(r=>r != null).Select(r=>r!).ToList(); // Filter out null results and return non-null UploadResult instances
        }
        public async Task<bool> DeleteFileAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(publicId)) return false;

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting {PublicId} from Cloudinary.", publicId);
                return false;
            }
        }

        public async Task<bool> DeleteFilesAsync(IEnumerable<string> publicIds)
        {
            var tasks = publicIds.Select(publicId => DeleteFileAsync(publicId));
            var results = await Task.WhenAll(tasks);
            return results.All(r => r); // Return true only if all deletions were successful
        }

    }
}
