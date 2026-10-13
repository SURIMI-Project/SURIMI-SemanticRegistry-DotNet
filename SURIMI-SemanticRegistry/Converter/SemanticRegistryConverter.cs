using Eii.ControlledVocabularies.Common;
using Eii.SemanticRegistry.Core;
using Eii.SemanticRegistry.DTO;
using Eii.SemanticRegistry.Serialization;
using SURIMI.SemanticRegistry.Dto;
using System.Text.Json;

namespace SURIMI.SemanticRegistry.Converter
{
    public class SemanticsRegistryConverter
    {
        public static DomainSemanticRegistry FromJSON(string jsonContent)
        {
            // Deserialize JSON
            SemanticFileDto? semanticsFile;
            try
            {
                semanticsFile = JsonSerializer.Deserialize<SemanticFileDto>(jsonContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse semantics file: {ex.Message}", ex);
            }

            if (semanticsFile == null)
            {
                throw new InvalidDataException($"Semantics file contains no valid data.");
            }
            return FromDTO(semanticsFile).FromDTO();
        }

        public static DomainSemanticRegistryDTO FromDTO(SemanticFileDto semanticsFile)
        {
            if (semanticsFile == null)
                throw new ArgumentNullException(nameof(semanticsFile));

            var result = new DomainSemanticRegistryDTO();

            foreach (SemanticDomainDto domain in semanticsFile.Domains)
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
