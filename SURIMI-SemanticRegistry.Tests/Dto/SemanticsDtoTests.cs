using FluentAssertions;
using SURIMI.SemanticRegistry.Dto;
using System.Text.Json;
using Xunit;

namespace SURIMI.SemanticRegistry.Tests.Dto
{
    public class SemanticsDtoTests
    {
        [Fact]
        public void NewDtos_HaveEmptyDefaults()
        {
            // Arrange & Act
            var file = new SemanticsFileDto();
            var domain = new SemanticsDomainDto();
            var mapping = new SemanticsMappingDto();

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
            var file = JsonSerializer.Deserialize<SemanticsFileDto>(json);

            // Assert
            file.Should().NotBeNull();
            file!.Domains.Should().ContainSingle();
            file.Domains[0].Domain.Should().Be("X");
            file.Domains[0].Mappings.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new SemanticsMappingDto { Source = "s", Target = "t" });
        }
    }
}
