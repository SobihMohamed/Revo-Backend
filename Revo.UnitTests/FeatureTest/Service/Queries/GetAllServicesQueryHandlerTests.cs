using Ardalis.Specification;
using Moq;
using Revo.Application.Contracts.Repositories;
using Revo.Application.Features.Services.Dto;
using Revo.Application.Features.Services.Queries.GetAll;
using Revo.Application.Features.Services.Queries.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Service.Queries
{
    public class GetAllServicesQueryHandlerTests
    {
        private readonly Mock<IGenericRepo<Domain.Entities.Service>> _serviceRepoMock;
        private readonly GetAllServicesQueryHandler _handler;

        public GetAllServicesQueryHandlerTests()
        {
            _serviceRepoMock = new Mock<IGenericRepo<Domain.Entities.Service>>();
            _handler = new GetAllServicesQueryHandler(_serviceRepoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnPaginatedResponse_When_DataExists()
        {
            // Arrange
            var specParams = new ServiceSpecParams
            {
                PageIndex = 1,
                PageSize = 2,
                Search = "Test"
            };
            var query = new GetAllServicesQuery(specParams);

            var expectedData = new List<ServiceDto>
            {
                new ServiceDto { Id = Guid.NewGuid(), NameAr = "خدمة 1" },
                new ServiceDto { Id = Guid.NewGuid(), NameAr = "خدمة 2" }
            };

            _serviceRepoMock.Setup(r => r.CountAsync(It.IsAny<ISpecification<Domain.Entities.Service>>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(5);

            _serviceRepoMock.Setup(r => r.ListAsync(It.IsAny<ISpecification<Domain.Entities.Service, ServiceDto>>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(expectedData);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            Assert.Equal(1, result.Value.PageIndex);
            Assert.Equal(2, result.Value.PageSize);
            Assert.Equal(5, result.Value.TotalCount); 

            Assert.Equal(2, result.Value.Data.Count);
            Assert.Equal("خدمة 1", result.Value.Data[0].NameAr);
        }

        [Fact]
        public async Task Handle_Should_ReturnEmptyList_When_NoDataMatches()
        {
            // Arrange
            var specParams = new ServiceSpecParams { PageIndex = 1, PageSize = 10 };
            var query = new GetAllServicesQuery(specParams);

            _serviceRepoMock.Setup(r => r.CountAsync(It.IsAny<ISpecification<Domain.Entities.Service>>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(0);

            _serviceRepoMock.Setup(r => r.ListAsync(It.IsAny<ISpecification<Domain.Entities.Service, ServiceDto>>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new List<ServiceDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Value.TotalCount);
            Assert.Empty(result.Value.Data);
        }
    }
}