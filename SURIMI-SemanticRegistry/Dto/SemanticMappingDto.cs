using System.Text.Json.Serialization;

namespace SURIMI.SemanticRegistry.Dto
{
    public sealed class SemanticMappingDto
    {
        [JsonPropertyName("Source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("Target")]
        public string Target { get; set; } = string.Empty;
    }
}
