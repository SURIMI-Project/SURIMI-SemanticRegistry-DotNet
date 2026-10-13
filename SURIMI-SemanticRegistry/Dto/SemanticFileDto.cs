using System.Text.Json.Serialization;

namespace SURIMI.SemanticRegistry.Dto
{
    public sealed class SemanticFileDto
    {
        [JsonPropertyName("Domains")]
        public List<SemanticDomainDto> Domains { get; set; } = new();
    }
}
