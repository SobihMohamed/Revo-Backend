
namespace Revo.Application.Abstraction.Services
{
    public record UploadResult(string Url, string PublicId);
    public interface IUploadService
    {
         Task<UploadResult?> UploadFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
         Task<IEnumerable<UploadResult>> UploadFilesAsync(IEnumerable<( Stream fileStream, string fileName)> files, CancellationToken cancellationToken = default);
         Task<bool> DeleteFileAsync(string publicId, CancellationToken cancellationToken = default);
         Task<bool> DeleteFilesAsync(IEnumerable<string> publicIds, CancellationToken cancellationToken = default);
    }
}
