using Eii.ControlledVocabularies.Common;
using FluentAssertions;
using SURIMI.SemanticRegistry.Converter;
using SURIMI.SemanticRegistry.Dto;
using System.Text.Json;
using Xunit;

namespace SURIMI.SemanticRegistry.Tests.Converter
{
    public class SemanticRegistryConverterTests
    {
        private static string FirstDomainName => Enum.GetNames(typeof(KeyDomain)).First();

        private static string ToJson(params (string Domain, (string Source, string Target)[] Mappings)[] domains)
        {
            var file = new SemanticFileDto();
            foreach (var d in domains)
            {
                var domain = new SemanticDomainDto { Domain = d.Domain };
                foreach (var m in d.Mappings)
                    domain.Mappings.Add(new SemanticMappingDto { Source = m.Source, Target = m.Target });
                file.Domains.Add(domain);
            }
            return JsonSerializer.Serialize(file);
        }

        [Fact]
        public void ToRegistryDTO_NullFile_ThrowsArgumentNullException()
        {
            // Arrange
            SemanticFileDto? file = null;

            // Act
            var act = () => SemanticRegistryConverter.FromDTO(file!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ToRegistryDTO_EmptyDomains_ReturnsEmptyRegistries()
        {
            // Arrange
            var file = new SemanticFileDto();

            // Act
            var result = SemanticRegistryConverter.FromDTO(file);

            // Assert
            result.Registries.Should().BeEmpty();
        }

        [Fact]
        public void ToRegistryDTO_InvalidDomain_ThrowsInvalidDataException()
        {
            // Arrange
            var file = new SemanticFileDto();
            file.Domains.Add(new SemanticDomainDto { Domain = "NotARealDomain_12345" });

            // Act
            var act = () => SemanticRegistryConverter.FromDTO(file);

            // Assert
            act.Should().Throw<InvalidDataException>().WithMessage("*NotARealDomain_12345*");
        }

        [Fact]
        public void ToRegistryDTO_ValidDomain_CopiesMappings()
        {
            // Arrange
            var file = new SemanticFileDto();
            var domain = new SemanticDomainDto { Domain = FirstDomainName };
            domain.Mappings.Add(new SemanticMappingDto { Source = "a", Target = "b" });
            domain.Mappings.Add(new SemanticMappingDto { Source = "c", Target = "d" });
            file.Domains.Add(domain);
            var key = Enum.Parse<KeyDomain>(FirstDomainName);

            // Act
            var result = SemanticRegistryConverter.FromDTO(file);

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
            var file = new SemanticFileDto();
            file.Domains.Add(new SemanticDomainDto { Domain = FirstDomainName.ToLowerInvariant() });

            // Act
            var result = SemanticRegistryConverter.FromDTO(file);

            // Assert
            result.Registries.Should().ContainKey(Enum.Parse<KeyDomain>(FirstDomainName));
        }

        [Fact]
        public void ToRegistryDTO_DuplicateDomains_Throws()
        {
            // Arrange
            var file = new SemanticFileDto();
            file.Domains.Add(new SemanticDomainDto { Domain = FirstDomainName });
            file.Domains.Add(new SemanticDomainDto { Domain = FirstDomainName });

            // Act
            var act = () => SemanticRegistryConverter.FromDTO(file);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void ToRegistryDTO_MalformedJson_ThrowsWrappedException()
        {
            // Arrange
            var json = "{ not valid json";

            // Act
            var act = () => SemanticRegistryConverter.FromJSON(json);

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
            var act = () => SemanticRegistryConverter.FromJSON(json);

            // Assert
            act.Should().Throw<InvalidDataException>();
        }

        [Fact]
        public void ToRegistryDTO_ValidJson_ReturnsRegistry()
        {
            // Arrange
            var json = ToJson((FirstDomainName, new[] { ("a", "b") }));

            // Act
            var result = SemanticRegistryConverter.FromJSON(json);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void ToRegistryDTO_InvalidDomainJson_ThrowsInvalidDataException()
        {
            // Arrange
            var json = ToJson(("Bogus_Domain_999", Array.Empty<(string, string)>()));

            // Act
            var act = () => SemanticRegistryConverter.FromJSON(json);

            // Assert
            act.Should().Throw<InvalidDataException>();
        }
    }
}
