# Design Notes

## Architecture

The package is split into Editor-only modules for UI, analyzers, planners, texture utilities, material utilities, mesh placeholders, mobile profiles, NDMF placeholders, and shared utilities.

## Non-destructive model

Analyzers should produce reports and plans. Transformers, when introduced later, should write generated assets or build-time replacements instead of mutating original source assets.

## Extension points

- `AvatarContext` will hold selected avatar data and cached scan results.
- Analyzer classes should expose dry-run entry points first.
- Planner classes should consume analyzer results and produce recommendations.
- NDMF integration should stay isolated under `Editor/NDMF`.

## Shader support

Poiyomi and lilToon support is not a first milestone. Shader-specific behavior should be implemented behind interfaces or adapters after reviewing shader APIs and material property conventions.
