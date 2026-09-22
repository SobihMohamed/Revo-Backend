using Moq;
using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Dto;
using Revo.Application.Features.Services.Commands.Create;
using Revo.Application.Features.Services.Specifications; 
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Revo.UnitTests.FeatureTest.Service.Commands.Create
{
    public class CreateServiceCommandHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Service>> _serviceRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUploadService> _uploadServiceMock;
        private readonly CreateServiceCommandHandler _handler;

        public CreateServiceCommandHandlerTests()
        {
            _serviceRepoMock = new Mock<IGenericRepo<Domain.Entities.Service>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _uploadServiceMock = new Mock<IUploadService>();

            _handler = new CreateServiceCommandHandler(
                _serviceRepoMock.Object,
                _uploadServiceMock.Object,
                _unitOfWorkMock.Object);
        }

        private ImageUploadDto CreateDummyImageDto()
        {
            var dummyStream = new MemoryStream(new byte[] { 1 });
            return new ImageUploadDto(dummyStream, "dummy.jpg");
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ServiceNameIsDuplicate()
        {
            // Arrange
            var imageDto = CreateDummyImageDto();
            var command = new CreateServiceCommand("خدمة", "Service", "وصف", "Desc", 1, imageDto);

            _serviceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ServiceByNameSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.Service());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service.DuplicateName", result.Error.Code); 

            _uploadServiceMock.Verify(u => u.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _serviceRepoMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Service>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ImageUploadFails()
        {
            // Arrange
            var imageDto = CreateDummyImageDto();
            var command = new CreateServiceCommand("خدمة", "Service", "وصف", "Desc", 1, imageDto);

            _serviceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ServiceByNameSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Service?)null);

            _uploadServiceMock.Setup(u => u.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UploadResult?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("UploadFailed", result.Error.Code);

            _serviceRepoMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Service>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessWithGuid_When_CommandIsValid()
        {
            // Arrange
            var imageDto = CreateDummyImageDto();
            var command = new CreateServiceCommand("خدمة", "Service", "وصف", "Desc", 1, imageDto);

            var fakeUploadResult = new UploadResult
            (
              "https://cloudinary.com/fake-url.png",
              "fake-public-id"
            );

            _serviceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ServiceByNameSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Service?)null);

            _uploadServiceMock.Setup(u => u.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeUploadResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            _serviceRepoMock.Verify(r => r.AddAsync(It.Is<Domain.Entities.Service>(s =>
                s.NameAr == "خدمة" &&
                s.ImageUrl == "https://cloudinary.com/fake-url.png" &&
                s.ImagePublicId == "fake-public-id"
            ), It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkMock.Verify(u => u.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}