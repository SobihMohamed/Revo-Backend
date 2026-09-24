using Moq;
using Revo.Application.Contracts.Identity;
using Revo.Application.Features.Auth.Commands.Login;
using Revo.Application.Features.Auth.Dto;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.UnitTests.FeatureTest.Auth.Command
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _handler = new LoginCommandHandler(_identityServiceMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessWithToken_When_CredentialsAreValid()
        {
            // Arrange
            var command = new LoginCommand("admin@revo.com", "Password@123");
            var expectedAuthResponse = new AuthResponse
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR...",
                IsAuthenticated = true,
                Email = command.Email,
                Name = "المدير"
            };

            _identityServiceMock
                .Setup(service => service.LoginAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<AuthResponse>.Success(expectedAuthResponse));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedAuthResponse.Token, result.Value.Token);
            Assert.Equal(expectedAuthResponse.Email, result.Value.Email);

            _identityServiceMock.Verify(service =>
                service.LoginAsync(command.Email, command.Password, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_CredentialsAreInvalid()
        {
            // Arrange
            var command = new LoginCommand("wrong@revo.com", "WrongPassword");
            var expectedError = new Error("Auth.Failed", "the email or password is incorrect");

            _identityServiceMock
                .Setup(service => service.LoginAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<AuthResponse>.Failure(expectedError));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedError.Code, result.Error.Code);

            _identityServiceMock.Verify(service =>
                service.LoginAsync(command.Email, command.Password, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}