# Texture最適化メモ

Texture参照一覧化は実装済みです。AvatarScanner / TextureUsageAnalyzerはShaderUtilでTexture propertyを走査し、寸法、Importer設定、概算memoryをレポートします。元Texture / Material / Meshは変更しません。UV領域解析や実最適化は将来作業です。

## 将来候補の機能

- UV Usage Analyzer: Mesh UVから使用されているTexture領域を推定します。
- Texture Waste Report: 使用UV領域とTexture寸法を比較します。
- Texture Region Pruner: 明示的なopt-in後に、切り抜いたコピーを生成します。
- Texture Repacker / Atlas Builder: 元Assetに触れず、新しいTextureと再マップ済みMaterialを生成します。

## リスク

- ShaderごとにTexture slotが異なります。
- UV setとMaterial slotの組み合わせは複雑です。
- Atlas generationでは、padding、mipmap、normal map、mask、color spaceの扱いに注意が必要です。
- `material.mainTexture` だけでは、BaseMap以外のNormal、Emission、Mask、MatCap、Ramp、Shader固有Textureを見落とす可能性があります。
- UV変更やTexture Atlas化は未実装です。将来はBuild cloneや生成物を使う安全設計と明示的opt-inを必要とします。
- 未使用Textureを勝手に削除しません。「未使用候補の検出」と「削除」は別物として扱います。
