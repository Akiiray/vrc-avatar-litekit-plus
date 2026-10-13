# 現行アーキテクチャ

LiteKitは診断・計画と、明示的に有効化された非破壊Build-time変換を扱います。現行v0.2でNDMFを利用する主要機能はParameter Compressorです。他の最適化機能が実装済みであることを意味しません。

## モジュールと責務

| 領域 | 現行の責務 |
| --- | --- |
| Analyzer | Avatar、Renderer、Material、Texture、Performance stats、Mobile互換性の読み取り・診断 |
| Planner | Performance Rank評価、目標との差分と改善計画。将来拡張用の型も含む |
| UI | 対象と条件の選択、診断サマリー、詳細、レポートコピー |
| Localization | 日本語 / Englishの表示切替。両Windowは同じ診断・計画処理を使う |
| Mobile diagnostics | Shader名等に基づく簡易診断。Material変換やComponent削除はしない |
| Runtime Component | Avatar Rootに設定するLiteKitParameterCompressor。IEditorOnlyであり、ゲーム内実行ロジックではない |
| Parameter Compression | Parameter走査、除外、決定的な計画、Inspector Preview、Encoder / Decoder生成 |
| NDMF integration | Editor/ParameterCompressionのPluginがOptimizing Phaseに参加 |
| 将来拡張 | UV / Mesh / Texture最適化等のPlaceholder。実変換とは区別する |

Editor / Runtime assemblyは各1つです。現在PackageとしてSDK / NDMFを必須依存に持ちますが、診断実行がNDMF Buildを開始するわけではありません。依存Versionは[package.json](../package.json)、変更履歴は[CHANGELOG](../CHANGELOG.md)を正本とします。

## 診断と計画の流れ

対象・条件選択 → AvatarScanner / AvatarContext → Analyzer → Performance評価・目標計画 → LiteKitReport → UI / Markdown / JSON。

診断は元Assetを変更しません。非アクティブGameObjectや無効Componentも対象です。Texture参照はShaderUtilでTexture propertyを走査します。Texture memoryは概算であり、RankやMobile判定もSDK Validationの代替保証ではありません。

Analyzer / Plannerの処理を言語別に二重実装しません。ユーザー向け表示と翻訳はLocalizationへ集約する方針です。Scanner / Mobile診断はコード・対象・Severityを保持し、UIとMarkdown表示時にLocalizationで翻訳します。JSONは診断コードを含むデータ出力です。

## Parameter Compressorの流れ

Avatar RootのRuntime Component → Inspectorで現在のExpression Parameters / FXを走査しPreview → 通常Build → NDMF Optimizing → Build時構成の再走査・計画 → cloneのExpression Parameters / FX Controllerを明示的に複製 → 変換・生成 → NDMF AssetSaver。

Componentの有効化初期値はtrueです。追加した時点でBuild時変換が有効になります。Inspector Previewは元Assetを変更する適用操作でも、実BuildのDry Runでもありません。前段Pluginによる変更後のBuild構成を正とします。

仕様、対象Parameter、同期方式、互換性の詳細は[PARAMETER_COMPRESSION](PARAMETER_COMPRESSION.md)を正本とします。LiteKit自身はSDK Build / Upload APIを呼びません。Modular AvatarはOptionalです。

## 非破壊モデル

非破壊とは元データを保持してcloneまたは生成物を変換することです。Build cloneで実現できる変換を優先し、元Prefab / Material / Texture / Mesh / Animator / Expression Parametersを直接変更しません。将来TransformerにもPreview、Before / After、明示的opt-inを設計します。詳細は[SAFETY_POLICY](SAFETY_POLICY.md)を参照してください。

Texture Atlas、UV変更、Mesh結合、ポリゴン削減、Material統合、Shader自動変換、Avatar Exporterは未実装です。NDMF一般Placeholderはこれらの将来設計の置き場であり、Compressor未実装を意味しません。

## 現行の制限と後続検証

- VRCFuryの確実なPlugin orderingは未確定です。推測でorderingを指定しません。
- Inspector PreviewはDescriptorのExpression Parameters / FX AnimatorControllerを取得元として説明します。NDMF ParameterInfoを使用するという説明はしません。
- Component追加時ONという現行挙動と、追加後ONを求める既存案内の差異は後続の仕様・文言整理候補です。
- 予約Parameter / Layer名の衝突、既存Animatorの型一致、clone参照の安全性、実ネットワーク同期は個別検証が必要です。
- Reportは診断対象Avatar参照・診断完了状態・Platform・目標Rankを保持します。対象または条件変更でstaleになり、再診断までサマリー・詳細・コピーを表示しません。言語・Foldout変更ではstaleにしません。
- Avatar内部の編集や外部Asset変更を自動検出する仕組みはありません。編集後は再診断してください。
- Severityに従ってInfo / Warning / Errorを表示し、Infoだけでは診断状態を警告にしません。

## 文書の役割

利用案内は[Documentation~](../Documentation~/README.md)、今後の範囲は[ROADMAP](ROADMAP.md)、歴史的制約と作業規則は[AGENTS](../AGENTS.md)を参照します。

UI詳細の正本は[Toolkit UIガイド](https://github.com/Akiiray/vrc-avatar-toolkit-plus/blob/main/Documentation~/UI_DESIGN_GUIDELINES.md)、横断責務は[VPMアーキテクチャ](https://github.com/Akiiray/vpm-repository/blob/main/docs/ECOSYSTEM_ARCHITECTURE.md)です。共通化のためにLocalizationやNDMF設計を撤去せず、Shared Packageを先行して新設しません。
