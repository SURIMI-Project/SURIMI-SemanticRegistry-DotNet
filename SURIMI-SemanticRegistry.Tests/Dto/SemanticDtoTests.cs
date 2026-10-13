using FluentAssertions;
using SURIMI.SemanticRegistry.Dto;
using System.Text.Json;
using Xunit;

namespace SURIMI.SemanticRegistry.Tests.Dto
{
    public class SemanticDtoTests
    {
        [Fact]
        public void NewDtos_HaveEmptyDefaults()
        {
            // Arrange & Act
            var file = new SemanticFileDto();
            var domain = new SemanticDomainDto();
            var mapping = new SemanticMappingDto();

            // Assert
            file.Domains.Should().BeEmpty();
            domain.Domain.Should().BeEmpty();
            domain.Mappings.Should().BeEmpty();
            mapping.Source.Should().BeEmpty();
            mapping.Target.Should().BeEmpty();
        }

        [Fact]
        public void Deserialize_ValidJson_MapsPropertyNames()
        {
            // Arrange
            var json = "{\"Domains\":[{\"Domain\":\"X\",\"Mappings\":[{\"Source\":\"s\",\"Target\":\"t\"}]}]}";

            // Act
            var file = JsonSerializer.Deserialize<SemanticFileDto>(json);

            // Assert
            file.Should().NotBeNull();
            file!.Domains.Should().ContainSingle();
            file.Domains[0].Domain.Should().Be("X");
            file.Domains[0].Mappings.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new SemanticMappingDto { Source = "s", Target = "t" });
        }
    }
}
