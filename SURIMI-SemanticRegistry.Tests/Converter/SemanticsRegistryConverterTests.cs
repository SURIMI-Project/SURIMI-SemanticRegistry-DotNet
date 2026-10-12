using Eii.ControlledVocabularies.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SURIMI.SemanticRegistry.Converter;
using SURIMI.SemanticRegistry.Dto;
using System.Text.Json;
using Xunit;

namespace SURIMI.SemanticRegistry.Tests.Converter
{
    public class SemanticsRegistryConverterTests
    {
        private readonly Mock<ILogger<SemanticsRegistryConverter>> _logger = new();

        private static string FirstDomainName => Enum.GetNames(typeof(KeyDomain)).First();

        private static string ToJson(params (string Domain, (string Source, string Target)[] Mappings)[] domains)
        {
            var file = new SemanticsFileDto();
            foreach (var d in domains)
            {
                var domain = new SemanticsDomainDto { Domain = d.Domain };
                foreach (var m in d.Mappings)
                    domain.Mappings.Add(new SemanticsMappingDto { Source = m.Source, Target = m.Target });
                file.Domains.Add(domain);
            }
            return JsonSerializer.Serialize(file);
        }

        [Fact]
        public void ToRegistryDTO_NullFile_ThrowsArgumentNullException()
        {
            // Arrange
            SemanticsFileDto? file = null;

            // Act
            var act = () => SemanticsRegistryConverter.FromDTO(file!, _logger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ToRegistryDTO_EmptyDomains_ReturnsEmptyRegistries()
        {
            // Arrange
            var file = new SemanticsFileDto();

            // Act
            var result = SemanticsRegistryConverter.FromDTO(file, _logger.Object);

            // Assert
            result.Registries.Should().BeEmpty();
        }

        [Fact]
        public void ToRegistryDTO_InvalidDomain_ThrowsInvalidDataException()
        {
            // Arrange
            var file = new SemanticsFileDto();
            file.Domains.Add(new SemanticsDomainDto { Domain = "NotARealDomain_12345" });

            // Act
            var act = () => SemanticsRegistryConverter.FromDTO(file, _logger.Object);

            // Assert
            act.Should().Throw<InvalidDataException>().WithMessage("*NotARealDomain_12345*");
        }

        [Fact]
        public void ToRegistryDTO_ValidDomain_CopiesMappings()
        {
            // Arrange
            var file = new SemanticsFileDto();
            var domain = new SemanticsDomainDto { Domain = FirstDomainName };
            domain.Mappings.Add(new SemanticsMappingDto { Source = "a", Target = "b" });
            domain.Mappings.Add(new SemanticsMappingDto { Source = "c", Target = "d" });
            file.Domains.Add(domain);
            var key = Enum.Parse<KeyDomain>(FirstDomainName);

            // Act
            var result = SemanticsRegistryConverter.FromDTO(file, _logger.Object);

            // Assert
            result.Registries.Should().ContainKey(key);
            var mappings = result.Registries[key].Registry.Mappings;
            mappings.Should().HaveCount(2);
            mappings.Select(m => (m.SourceCanonical, m.TargetCanonical))
                .Should().Equal(("a", "b"), ("c", "d"));
        }

        [Fact]
        public void ToRegistryDTO_LowerCaseDomain_ParsesCaseInsensitively()
        {
            // Arrange
            var file = new SemanticsFileDto();
            file.Domains.Add(new SemanticsDomainDto { Domain = FirstDomainName.ToLowerInvariant() });

            // Act
            var result = SemanticsRegistryConverter.FromDTO(file, _logger.Object);

            // Assert
            result.Registries.Should().ContainKey(Enum.Parse<KeyDomain>(FirstDomainName));
        }

        [Fact]
        public void ToRegistryDTO_DuplicateDomains_Throws()
        {
            // Arrange
            var file = new SemanticsFileDto();
            file.Domains.Add(new SemanticsDomainDto { Domain = FirstDomainName });
            file.Domains.Add(new SemanticsDomainDto { Domain = FirstDomainName });

            // Act
            var act = () => SemanticsRegistryConverter.FromDTO(file, _logger.Object);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void ToRegistryDTO_MalformedJson_ThrowsWrappedException()
        {
            // Arrange
            var json = "{ not valid json";

            // Act
            var act = () => SemanticsRegistryConverter.FromJSON(json, _logger.Object);

            // Assert
            act.Should().Throw<Exception>()
                .WithMessage("Failed to parse semantics file:*")
                .WithInnerException<JsonException>();
        }

        [Fact]
        public void ToRegistryDTO_NullJson_ThrowsInvalidDataException()
        {
            // Arrange
            var json = "null";

            // Act
            var act = () => SemanticsRegistryConverter.FromJSON(json, _logger.Object);

            // Assert
            act.Should().Throw<InvalidDataException>();
        }

        [Fact]
        public void ToRegistryDTO_ValidJson_ReturnsRegistry()
        {
            // Arrange
            var json = ToJson((FirstDomainName, new[] { ("a", "b") }));

            // Act
            var result = SemanticsRegistryConverter.FromJSON(json, _logger.Object);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void ToRegistryDTO_InvalidDomainJson_ThrowsInvalidDataException()
        {
            // Arrange
            var json = ToJson(("Bogus_Domain_999", Array.Empty<(string, string)>()));

            // Act
            var act = () => SemanticsRegistryConverter.FromJSON(json, _logger.Object);

            // Assert
            act.Should().Throw<InvalidDataException>();
        }
    }
}
