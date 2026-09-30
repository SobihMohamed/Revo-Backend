using Revo.Application.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.IntegrationTests.Infrastructre.FakeExternalService
{
    public class FakeUploadService : IUploadService
    {
        public Task<UploadResult?> UploadFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<UploadResult?>(new UploadResult
            (
                $"https://fakeuploadservice.com/{fileName}",
                Guid.NewGuid().ToString()
            ));
        }

        public Task<IEnumerable<UploadResult>> UploadFilesAsync(IEnumerable<(Stream fileStream, string fileName)> files, CancellationToken cancellationToken = default)
        {
            var results = files.Select(f => new UploadResult
            (
                $"https://fake-cdn.com/images/{Guid.NewGuid()}_{f.fileName}",
                $"fake_public_id_{Guid.NewGuid()}"
            )).ToList().AsEnumerable();

            return Task.FromResult(results);
        }
        public Task<bool> DeleteFileAsync(string publicId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteFilesAsync(IEnumerable<string> publicIds)
        {
            return Task.FromResult(true);
        }

    }
}
