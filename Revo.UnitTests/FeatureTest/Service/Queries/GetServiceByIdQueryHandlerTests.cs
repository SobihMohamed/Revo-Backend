using Ardalis.Specification;
using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Services.Dto;
using Revo.Application.Features.Services.Queries.GetById;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Service.Queries
{
    public class GetServiceByIdQueryHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Service>> _serviceRepoMock;
        private readonly GetServiceByIdQueryHandler _handler;

        public GetServiceByIdQueryHandlerTests()
        {
            _serviceRepoMock = new Mock<IGenericRepo<Domain.Entities.Service>>();
            _handler = new GetServiceByIdQueryHandler(_serviceRepoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessWithDto_When_ServiceExists()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var query = new GetServiceByIdQuery(serviceId);

            var expectedDto = new ServiceDto
            {
                Id = serviceId,
                NameAr = "خدمة تصميم",
                NameEn = "Design Service"
            };

            _serviceRepoMock.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<ISpecification<Domain.Entities.Service, ServiceDto>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(serviceId, result.Value.Id);
            Assert.Equal("خدمة تصميم", result.Value.NameAr);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ServiceDoesNotExist()
        {
            // Arrange
            var query = new GetServiceByIdQuery(Guid.NewGuid());

            _serviceRepoMock.Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<ISpecification<Domain.Entities.Service, ServiceDto>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((ServiceDto?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Service.NotFound", result.Error.Code);
        }
    }
}