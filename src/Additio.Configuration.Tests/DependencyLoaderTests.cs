using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Additio.Configuration.Tests
{
    public class DependencyLoaderTests
    {
        [Theory]
        [AutoMockedData("Sitecore.Analytics", 1)]
        [AutoMockedData("Sitecore.*", 1)]
        [AutoMockedData("Sitecore.*, Custom.*", 2)]
        [AutoMockedData("Sitecore.*, Custom.*.config", 2)]
        public void GetDependencyPatterns_ShouldAddMissingConfigExtension(string dependencies, int dependencyCount, string file)
        {
            // Arrange
            var attributes = new[] { new XAttribute("dependencies", dependencies) };
            var attributeLoader = Substitute.For<IAttributeLoader>();
            attributeLoader.GetAttributesOnRootElement(file).Returns(attributes);

            // Act
            var loader = new DependencyLoader(attributeLoader);
            var patterns = loader.GetDependencyPatterns(file).ToList();

            // Assert
            patterns.Should().HaveCount(dependencyCount);
            foreach (var pattern in patterns)
            {
                pattern.Should().EndWithEquivalent(".config");
            }
        }
    }
}
