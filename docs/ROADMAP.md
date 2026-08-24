# ロードマップ

## Phase 0: Scaffold

- Package metadataとUnity assembly definitionの整備。
- Editor window skeletonの追加。
- 安全方針、設計メモ、Issue草案の整備。
- package rootがリポジトリ直下にある構造を維持。

## Phase 1: Diagnostics

- Avatar選択と `AvatarContext` の拡充。
- Dry Run analyzer logの整備。
- Material duplicate detection report。
- 元アセットを変更しないTexture / UV usage report。
- Performance Rankの推定表示と、目標Rankとの差分表示。

## Phase 2: Planning

- PC / Quest / Android / iOS向けのPerformance Target Planner。
- Mobile texture profileの推奨事項。
- Before / After estimation report。
- Mobile向け警告と改善提案の整理。

## Phase 3: Build-time integration

- Parameter CompressorのNDMF `Optimizing` build pass（v0.2.0で実装）。
- Float / Intの共有同期チャンネル、Encoder / Decoder、Inspector preview（v0.2.0で実装）。
- ビルド時の生成Asset差し替え。
- 明示的なopt-inとDry Run preview。
- Avatar Optimizer、TexTransTool、Modular Avatarなどとの関係整理。

Parameter Compressor以外の自動変換は引き続きDeferredです。Puppetなど高頻度Parameterの個別方針と、VRCFuryとの確実なPlugin orderingは将来検証します。

## Deferred

- Texture region pruning。
- Atlas generation。
- UV relocation。
- Mesh merging。
- Polygon reduction。
- Material統合やShader変換の自動実行。
