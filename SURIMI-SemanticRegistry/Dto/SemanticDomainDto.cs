using System.Text.Json.Serialization;

namespace SURIMI.SemanticRegistry.Dto
{
    public sealed class SemanticDomainDto
    {
        [JsonPropertyName("Domain")]
        public string Domain { get; set; } = string.Empty;

        [JsonPropertyName("Mappings")]
        public List<SemanticMappingDto> Mappings { get; set; } = new();
    }
}