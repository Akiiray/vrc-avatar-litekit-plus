# VRC Avatar LiteKit Plus

`VRC Avatar LiteKit Plus` is an early-stage VPM / UPM package for VRChat avatar optimization planning and mobile conversion assistance.

The project is intended as a sibling project to `vrc-avatar-toolkit-plus`. Its design favors non-destructive workflows similar in spirit to build-time transformation systems such as NDMF and Modular Avatar.

## Status

This repository is in the initial development stage. The current package contains structure, documentation, menu integration, and safe placeholders only. Full optimization algorithms are intentionally not implemented yet.

## Safety-first goals

- Do not directly modify original avatars, Prefabs, Materials, Textures, or Meshes.
- Prefer diagnostics, planning, Dry Run output, and Before/After logs.
- Keep destructive or irreversible operations out of automatic flows.
- Require explicit confirmation for any future destructive operation.

## v0.1 diagnostic features

- Avatar Root selection from ObjectField or current Unity selection.
- Dry Run performance estimate based on public VRChat Performance Rank thresholds.
- PC / Mobile target platform and target rank comparison.
- Renderer, Material, Texture, and Mobile Warning tabs.
- Markdown and JSON report copy actions.

## Planned features

- Avatar Performance Analyzer
- Performance Target Planner
- Texture Usage Analyzer
- UV Usage Analyzer
- Texture Region Pruner
- Texture Repacker / Atlas Builder
- Material Duplicate Finder
- Mobile Texture Profile
- Quest / Android / iOS conversion assistant
- NDMF build-time optimization

## Non-goals for the initial version

- No polygon reduction implementation.
- No complete Poiyomi / lilToon conversion.
- No texture atlas generation implementation.
- No direct deletion of original assets.

## Future dependencies

VRChat SDK, NDMF, and Modular Avatar may be integrated in future versions after API usage is reviewed against official documentation. They are not direct dependencies in the initial package metadata.
