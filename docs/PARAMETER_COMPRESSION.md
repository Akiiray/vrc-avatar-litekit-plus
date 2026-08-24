# Parameter Compression

## 概要

Parameter Compressorは、VRChat Expression Parametersの同期Float / Intを共有チャンネルへ順番にCopyする時分割多重化機能です。値の量子化や数学的packingは行いません。Boolは1bitのため対象外です。

## 使用方法

1. Avatar Rootへ `VRC Avatar LiteKit Plus/Parameter Compressor` Componentを追加します。
2. Inspectorで「圧縮を有効にする」をONにします。
3. 同期チャンネル数と、必要に応じて圧縮除外Parameterを設定します。
4. 通常どおりVRChat AvatarをBuildします。

NDMFの`Optimizing` Phaseで自動処理されます。VRChat SDK Build / Upload APIをLiteKit自身が呼ぶことはありません。

## 同期方式

Float 5個、同期チャンネル数1の場合、元の40bitに対して次の16bitを生成します。

```text
LiteKit_PC_Sync_Index    Int    8bit
LiteKit_PC_Sync_Float_0 Float  8bit
```

Local Avatarは0.1秒ごとに元ParameterからチャンネルへCopyし、対応するIndexをSetします。Remote Avatarは受信Indexに応じてチャンネルから元ParameterへCopyします。複数Batchは連続巡回し、1 Batchだけの場合も`Batch_1 → LocalIdle → Batch_1`で再送します。

圧縮後コストは次の式です。

```text
8 + min(K, Float数) * 8 + min(K, Int数) * 8
```

元コストより小さくならない場合はBuild時に処理しません。Kは削減効果が残る最大値へ自動Clampされます。

## 非破壊性

NDMF Build clone上のExpression ParametersとFX AnimatorControllerを明示的に複製してから変更します。元Parameterは削除せず`networkSynced=false`へ切り替えるだけなので、名前、型、`saved`、`defaultValue`を維持します。Project内の元AssetへSave、Import、Deleteは行いません。

## 除外と互換性

次は自動除外します。

- Bool、VRChat Built-in / Reserved Parameter
- `LiteKit_PC_Sync_`で始まるLiteKit自身のチャンネル
- Unified Expressions v2形式のFace Tracking Parameter
- `LightLimitChanger/System/`および検出可能なVRCFury同期用Parameter
- ユーザーがInspectorで除外したParameter

MenuBuilderの同期Parameterを検出した場合は併用警告を表示しますが、他ツールの設定は変更しません。Modular Avatarは必須ではありません。

VRCFuryより前に実行することが目標ですが、公開された確実なNDMF Plugin QualifiedNameをこの実装では確認できていないため、推測によるordering指定はしていません。導入環境での実行順確認が必要です。

## Previewの制限

InspectorはExpression ParametersとFX AnimatorControllerから取得できる値を表示します。Build時には前段Plugin処理後の`descriptor.expressionParameters`を再走査するため、Inspectorの推定と異なる場合があります。Build結果を正としてください。PC / Android / iOS間で同期する場合は、対象ParameterとKを同じにしてください。
