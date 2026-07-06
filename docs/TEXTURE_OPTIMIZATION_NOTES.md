# Texture最適化メモ

Texture関連機能は将来作業です。初期コードは、分析stubとレポートに限定します。v0.1系では元Texture、元Material、元Meshを変更しません。

## 将来候補の機能

- Texture Usage Analyzer: RendererとMaterialからTexture参照を一覧化します。
- UV Usage Analyzer: Mesh UVから使用されているTexture領域を推定します。
- Texture Waste Report: 使用UV領域とTexture寸法を比較します。
- Texture Region Pruner: 明示的なopt-in後に、切り抜いたコピーを生成します。
- Texture Repacker / Atlas Builder: 元Assetに触れず、新しいTextureと再マップ済みMaterialを生成します。

## リスク

- ShaderごとにTexture slotが異なります。
- UV setとMaterial slotの組み合わせは複雑です。
- Atlas generationでは、padding、mipmap、normal map、mask、color spaceの扱いに注意が必要です。
- `material.mainTexture` だけでは、BaseMap以外のNormal、Emission、Mask、MatCap、Ramp、Shader固有Textureを見落とす可能性があります。
- UV変更やTexture Atlas化は破壊リスクが高いため、v0.1系では実行しません。
- 未使用Textureを勝手に削除しません。「未使用候補の検出」と「削除」は別物として扱います。
