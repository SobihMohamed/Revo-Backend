using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Revo.Infrastructure.ServicesImplementation;
using Revo.Infrastructure.Settings; 

namespace Revo.UnitTests.Infrastructure
{
    public class CloudinaryServiceTests
    {
        private readonly Mock<ILogger<CloudinaryService>> _loggerMock;
        private readonly Mock<IOptions<CloudinarySettings>> _optionsMock;
        private readonly CloudinaryService _sut; // System Under Test
        public CloudinaryServiceTests()
        {
            _loggerMock = new Mock<ILogger<CloudinaryService>>();
            _optionsMock = new Mock<IOptions<CloudinarySettings>>();

            _optionsMock.Setup(x => x.Value)
                .Returns(new CloudinarySettings
            {
                CloudName = "test_cloud",
                ApiKey = "test_key",
                ApiSecret = "test_secret"
            });

            _sut = new CloudinaryService(_loggerMock.Object, _optionsMock.Object);
        }
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task DeleteFileAsync_WhenPublicIdIsNullOrWhitespace_ShouldReturnFalse(string invalidPublicId)
        {
            // Act
            var result = await _sut.DeleteFileAsync(invalidPublicId);

            // Assert
            result.Should().BeFalse();

            // Verify that the logger was not called
            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(), // We don't care about the log level in this test
                    It.IsAny<EventId>(), // We don't care about the event ID in this test
                    It.IsAny<It.IsAnyType>(), // We don't care about the state in this test
                    It.IsAny<Exception>(), // We don't care about the exception in this test
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), // We don't care about the formatter in this test
                Times.Never);
        }
        [Fact]
        public async Task UploadFilesAsync_WhenGivenEmptyList_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<(Stream fileStream, string fileName)>();

            // Act
            var result = await _sut.UploadFilesAsync(emptyList);
            // Assert
            result.Should().BeEmpty();
        }
        [Fact]
        public async Task DeleteFilesAsync_WhenGivenEmptyList_ShouldReturnTrue()
        {
            // Arrange
            var emptyList = new List<string>();

            // Act
            var result = await _sut.DeleteFilesAsync(emptyList);

            // Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UploadFileAsync_WhenStreamIsNull_ShouldReturnNull()
        {
            // Arrange
            Stream nullStream = null!;
            string validFileName = "image.png";

            // Act
            var result = await _sut.UploadFileAsync(nullStream, validFileName);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task UploadFileAsync_WhenFileNameIsNullOrWhitespace_ShouldReturnNull(string invalidFileName)
        {
            // Arrange
            using var validStream = new MemoryStream(new byte[] { 1, 2, 3 }); // Fake Stream

            // Act
            var result = await _sut.UploadFileAsync(validStream, invalidFileName);

            // Assert
            result.Should().BeNull();
        }
    }
}
