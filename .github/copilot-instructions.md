# Copilot Instructions

## Project
- `SURIMI-SemanticRegistry` (`SURIMI.SemanticRegistry`): .NET 10 class library that converts SURIMI semantics JSON files into Eii semantic registry DTOs.
- `SURIMI-SemanticRegistry.Tests`: unit tests. Internals are exposed to it via `InternalsVisibleTo`.
- Licensed under EUPL-1.2.

## Coding conventions
- C# with nullable reference types and implicit usings enabled.
- DTOs live in `Dto/` and are `internal sealed`; JSON names are set with `[JsonPropertyName]`.
- Block-scoped namespaces, matching the existing files.
- Keep changes minimal; do not add comments unless they explain something non-obvious.

## Unit tests
- Frameworks: xunit 2.5.3, Moq 4.18.0, FluentAssertions 6.6.0.
- Mirror the library folder layout (`Converter/`, `Dto/`).
- Name tests `Method_Scenario_ExpectedResult`.
- Structure every test with `// Arrange`, `// Act` and `// Assert` comments (combine as `// Arrange & Act` only when unavoidable).
- Use FluentAssertions for assertions and Moq for dependencies such as `ILogger<T>`.
- Add tests for every new public or internal behavior, including error paths.
