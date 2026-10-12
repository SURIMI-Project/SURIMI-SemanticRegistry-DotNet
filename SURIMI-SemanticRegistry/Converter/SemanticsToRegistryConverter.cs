using Dto;
using Eii.ControlledVocabularies.Common;
using Eii.SemanticRegistry.Core;
using Eii.SemanticRegistry.DTO;
using Eii.SemanticRegistry.Serialization;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Converter
{
    public class SemanticsToRegistryConverter
    {
        public static DomainSemanticRegistry ToRegistryDTO(string jsonContent, ILogger<SemanticsToRegistryConverter> logger)
        {
            // Deserialize JSON
            SemanticsFileDto? semanticsFile;
            try
            {
                semanticsFile = JsonSerializer.Deserialize<SemanticsFileDto>(jsonContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse semantics file: {ex.Message}", ex);
            }

            if (semanticsFile == null)
            {
                throw new InvalidDataException($"Semantics file contains no valid data.");
            }
            return ToRegistryDTO(semanticsFile, logger).FromDTO();
        }

        internal static DomainSemanticRegistryDTO ToRegistryDTO(SemanticsFileDto semanticsFile, ILogger<SemanticsToRegistryConverter> logger)
        {
            if (semanticsFile == null)
                throw new ArgumentNullException(nameof(semanticsFile));

            var result = new DomainSemanticRegistryDTO();

            foreach (SemanticsDomainDto domain in semanticsFile.Domains)
            {
                if (!Enum.TryParse(domain.Domain, true, out KeyDomain domainParsed) ||
                    !Enum.IsDefined(typeof(KeyDomain), domainParsed))
                {
                    throw new InvalidDataException($"Invalid semantic domain: {domain.Domain}");
                }

                var registry = new SemanticRegistryDTO
                {
                    Domain = domainParsed
                };

                foreach (var mapping in domain.Mappings)
                {
                    registry.Registry.Mappings.Add(new CanonicalMappingDTO
                    {
                        SourceCanonical = mapping.Source,
                        TargetCanonical = mapping.Target
                    });
                }

                result.Registries.Add(domainParsed, registry);
            }

            return result;
        }
    }
}
