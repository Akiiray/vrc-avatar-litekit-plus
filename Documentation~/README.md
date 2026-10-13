# VRC Avatar LiteKit Plus

VRChatアバターの診断、Performance Rank目標計画、Mobile対応支援、非破壊Build-time変換を扱うツールです。現行v0.2ではParameter Compressorを提供します。

## 診断Window

- 日本語: `Tools > VRC Avatar LiteKit Plus > 日本語ウィンドウ`
- English: `Tools > VRC Avatar LiteKit Plus > English Window`

両Windowは同じAnalyzer / Plannerを使い、Localizationで表示を切り替えます。対象AvatarとPC / Mobile目標を選び、診断を実行するとサマリー、Renderer / Material / Texture、Mobile警告、Markdown / JSONレポートを確認できます。

診断はDry Runであり、元Avatar / Prefab / Material / Texture / Meshを変更しません。Performance RankとTexture memoryは推定、Mobile Shader判定は簡易判定です。SDK BuilderのValidation結果と完全一致する保証はありません。

## Parameter Compressor

Avatar Rootへ`VRC Avatar LiteKit Plus/Parameter Compressor`を追加し、チャンネル数と除外をInspectorで設定します。現行Componentは追加時から有効です。必要に応じて無効化できます。Previewでbit使用量、Batch数、推定同期周期を確認し、通常どおりBuildします。

Inspector Previewは現在のExpression Parameters / FXからの推定です。診断のDry Runとは異なり、有効なComponentは実Build時に変換されます。前段Plugin処理後のBuild構成が最終結果を決めます。

VRChat SDK3 - AvatarsとNDMFが必須です。VCC / ALCOM向けの依存はpackage.jsonのvpmDependenciesへ宣言します。UPMから直接参照する場合は、これらのVPM依存を別途導入する必要があります。Runtime Componentが設定を保持し、NDMF PluginがOptimizing PhaseでBuild cloneのExpression Parameters / FX Controllerを複製して変換し、Encoder / Decoderを生成します。元Assetを上書きせず、LiteKit自身はSDK Build / Uploadを開始しません。Modular Avatarは必須ではありません。

詳細な仕様と互換性は[Parameter Compression](../docs/PARAMETER_COMPRESSION.md)を参照してください。既存の「追加後ON」の案内とは初期値の説明に差異があり、実装の変更は行っていません。

## 現行範囲と将来機能

Analyzer、Planner、UI、Localization、Mobile diagnosticsに加えて、Parameter CompressionのRuntime Component / NDMF integrationが存在します。NDMFを利用する主要機能は現在Compressorです。

Texture Atlas化、UV変更、Mesh結合、ポリゴン削減、Material / Shader変換、Unsupported Componentの自動削除は未実装です。将来機能のPlaceholderと実装済みCompressorを区別します。v0.1の診断限定スコープは歴史的な制約です。

非破壊は一切の変更を禁じる意味ではなく、元データを保護してBuild cloneや生成物を変換する設計です。

## 開発資料

- [現行設計](../docs/DESIGN.md)
- [安全方針](../docs/SAFETY_POLICY.md)
- [ロードマップ](../docs/ROADMAP.md)
- [作業方針](../AGENTS.md)
- [Plus UI詳細規約](https://github.com/Akiiray/vrc-avatar-toolkit-plus/blob/main/Documentation~/UI_DESIGN_GUIDELINES.md)
- [Plusシリーズと配布](https://github.com/Akiiray/vpm-repository/blob/main/docs/ECOSYSTEM_ARCHITECTURE.md)
