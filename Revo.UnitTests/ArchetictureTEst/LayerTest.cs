using FluentAssertions;
using NetArchTest.Rules;
namespace Revo.UnitTests.ArchetictureTest
{
    public class LayerTest
    {
        private const string Application = "Revo.Application";
        private const string Infrastructure = "Revo.Infrastructure";
        private const string API = "Revo.API";
        [Fact]
        public void Domain_Should_Not_HaveDependencyOnOtherProjects()
        {
            // Arrange
            var assembly = typeof(Domain.DomainMarker).Assembly;

            var otherLayers = new[]
            {
                Application,
                Infrastructure,
                API
            };

            // Act 
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherLayers)
                .GetResult();

            // Assert
            result.IsSuccessful.Should().BeTrue("Domain should not have dependencies on other projects");

        }
        [Fact]
        public void ApplicationLayer_ShouldDependOnlyOnDomainLayer()
        {
            // Arrange
            var assembly = typeof(Application.Class1).Assembly;
            var otherLayers = new[]
            {
                Infrastructure,
                API
            };
            // Act 
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherLayers)
                .GetResult();
            // Assert
            result.IsSuccessful.Should().BeTrue("Application should only have dependencies on the Domain layer");
        }
        [Fact]
        public void InfrastructureLayer_ShouldDependOnlyOnDomainAndApplicationLayers()
        {
            // Arrange
            var assembly = typeof(Revo.Infrastructure.ServicesImplementation.CloudinaryService).Assembly;
            var otherLayers = new[]
            {
                API
            };
            // Act 
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherLayers)
                .GetResult();
            // Assert
            result.IsSuccessful.Should().BeTrue("Infrastructure should only have dependencies on the Domain and Application layers");
        }
    }
}
