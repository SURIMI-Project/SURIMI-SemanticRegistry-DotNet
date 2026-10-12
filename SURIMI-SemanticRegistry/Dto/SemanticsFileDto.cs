using System.Text.Json.Serialization;

namespace Dto
{
    internal sealed class SemanticsFileDto
    {
        [JsonPropertyName("Domains")]
        public List<SemanticsDomainDto> Domains { get; set; } = new();
    }
}
