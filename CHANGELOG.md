# Changelog

## [0.2.0] - 2026-08-24

### Added

- 同期Float / Intを共有チャンネルで時分割送信するParameter Compressorを追加しました。
- 同期チャンネル数、Parameter除外、bit削減量、Batch数、推定遅延を表示する日本語Inspectorを追加しました。
- NDMF `Optimizing` PhaseでBuild cloneだけを変更するEncoder / Decoder生成処理を追加しました。
- Built-in、Face Tracking、LiteKit自身、LightLimitChanger、VRCFury同期Parameterの自動除外とMenuBuilder併用警告を追加しました。

### Changed

- VRChat SDK3 - Avatars 3.7.0とNDMF 1.5.0を必須依存にしました。Modular Avatarは必須ではありません。

## [0.1.1] - 2026-07-02

### Fixed

- VPM Release asset向けのpackage root構造を修正しました。
- package layoutから古い入れ子の `Packages/` ディレクトリを削除しました。
- asmdefとpackage rootが重複する問題を解消しました。
- GitHub source archiveではなく手動添付したRelease assetを使うことで、VPM導入時に古いpackage layoutが復元されないようにしました。

## [0.1.0] - 2026-06-30

### Added

- 初期のVPM / UPM package metadataを追加しました。
- `Tools/VRC Avatar LiteKit Plus/Open LiteKit Window` にEditor windowを追加しました。
- 非アクティブなGameObjectとComponentを含める、非破壊方針のアバタースキャナーを追加しました。
- 公開されているVRChat Performance Rank閾値に基づくDry Runの推定診断を追加しました。
- Renderer、Material、Texture、Mobile互換性の診断レポートを追加しました。
- Markdown / JSON形式のレポートコピー機能を追加しました。
- 将来の安全な拡張に向けたAnalyzerとPlannerのプレースホルダーを追加しました。
- 安全優先のドキュメントとロードマップを追加しました。
