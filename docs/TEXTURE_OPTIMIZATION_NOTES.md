# Texture Optimization Notes

Texture features are future work. Initial code must remain limited to analysis stubs and reports.

## Candidate future features

- Texture Usage Analyzer: list texture references from renderers and materials.
- UV Usage Analyzer: estimate sampled texture regions by mesh UVs.
- Texture Waste Report: compare used UV regions with texture dimensions.
- Texture Region Pruner: generate cropped copies only after explicit opt-in.
- Texture Repacker / Atlas Builder: generate new textures and remapped materials without touching originals.

## Risks

- Shader-specific texture slots vary by shader.
- UV sets and material slots can be complex.
- Atlas generation requires careful handling of padding, mipmaps, normal maps, masks, and color spaces.
