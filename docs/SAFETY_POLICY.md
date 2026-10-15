# 安全方針

## 保護するデータ

元Avatarを不用意に変更せず、元Prefab / Material / Texture / Mesh / Animator / Expression Parametersを直接変更・削除・上書きしません。診断と変更を混同せず、未使用候補を自動削除しません。AssetDatabase.DeleteAssetは原則使いません。

## 非破壊変換

非破壊は一切変更しないという意味ではありません。元データを保持し、Build cloneで実現できる変換はBuild-time変換を優先します。別Assetを生成する場合も対象と出力を明確にし、Save / Importは生成物への操作であることを示します。

現行Parameter CompressorはNDMF OptimizingでBuild cloneのExpression Parameters / FX Controllerを複製して変更し、生成ObjectをNDMF AssetSaverで保存します。元AssetへのSave / Import / Deleteを行いません。詳細仕様は[PARAMETER_COMPRESSION](PARAMETER_COMPRESSION.md)を参照してください。

## 診断・Preview・確認

- 診断は読み取りとレポートを扱い、Dry Runは変更予定を示して変更を実行しません。
- Inspector Previewは現在構成からの推定です。有効Componentによる実Buildの変換を止める操作ではありません。
- 変換は明示的opt-inを必要とし、対象、影響、Before / After、推定の限界を説明します。現在Compressorは追加時から有効です。
- 破壊的・復元困難な操作を将来検討する場合は明示的な確認を必須とし、無言で実行しません。
- 構成不備やエラーを黙ってFallbackで隠しません。Skipする理由と結果を表示します。

## Buildと依存

LiteKit自身はSDK Build / Upload / MultiPlatform Uploadを勝手に呼びません。既存NDMF Passへの参加はBuild APIの起動とは異なります。SDK / NDMFは現行Compressorの必須依存であり、新規依存追加や内部APIへのReflectionは事前確認します。

## 将来機能と歴史

Texture / Mesh / Material変換等は未実装です。追加には個別の安全設計、Preview、Before / After、opt-inが必要です。v0.1では診断限定でNDMF本実装をスコープ外としましたが、v0.2のCompressorは正式な非破壊変換です。

実装範囲・既知の検証課題は[DESIGN](DESIGN.md)、開発・Releaseの制限は[AGENTS](../AGENTS.md)を正本とします。
