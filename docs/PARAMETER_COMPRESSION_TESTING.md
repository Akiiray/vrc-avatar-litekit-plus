# Parameter Compressor 検証記録

## NDMF AvatarProcessor の再現

Unity **2022.3.22f1**、VRChat SDK Base / Avatars **3.10.6**、NDMF **1.15.1**。Unity Editor のバッチモードから `AvatarProcessor.ProcessAvatar` を実行します。

1. 使い捨てのUnityプロジェクトに上記依存とPR #15のLiteKitを導入します。テストフレームワークや他の最適化Pluginは不要です。
2. [テストコード](../.github/verification/LiteKitContract.cs.txt)を一時プロジェクトの `Assets/Editor/LiteKitContract.cs` にコピーします。テストコードは配布対象外の `.github/verification/` にテキストとして保管しています。
3. Unity Editorを閉じ、以下を実行します。テストは実行ごとに別名の `Assets/Fixture_*` にAssetを作成します。

```powershell
$unity = 'D:\Unity\Hub\Editor\2022.3.22f1\Editor\Unity.exe'
$project = 'D:\Temp\LiteKitVerification'
$run = Start-Process -FilePath $unity -ArgumentList @(
    '-batchmode', '-nographics',
    '-projectPath', ('"' + $project + '"'),
    '-executeMethod', 'LiteKitContract.Run',
    '-logFile', ('"' + (Join-Path $project 'contract.log') + '"')
) -Wait -PassThru
if ($run.ExitCode -ne 0) { throw "Unity exited with $($run.ExitCode)" }
Select-String -Path (Join-Path $project 'contract.log') -Pattern '^LITEKIT_(CASE_PASS|CONTRACT_PASS) '
```

各ケースでPrefab、カスタムFX Controller、Expression Parametersを作り、Prefabのクローンを処理します。表のFはFloat、IはIntです。通常の入力は同期あり、savedあり、default 1です。Kは設定するチャンネル数で、生成数にはIndexを含みます。

| ケース | 入力 | 期待値 | 実測 |
|---|---|---|---|
| Float | 5F、K=1 | 生成2、5 Batch | 合格 |
| Int | 5I、K=1 | 生成2、5 Batch | 合格 |
| Mixed | 4F + 3I、K=2、独立したクローン2回 | 各回で生成5、4 Batch、追加Layer 1本 | 合格 |
| Zero | 0件、K=1 | 成功、生成なし | 合格 |
| Single | 1F、K=1 | 成功、削減なし、生成なし | 合格 |
| MaxUseful | 32F、K=999 | 実効K=30、生成31、2 Batch | 合格 |
| IndexMax | 255F、K=1 | 生成2、255 Batch | 合格 |
| IndexOverflow | 256F、K=1 | Batch上限で失敗 | 合格 |
| BudgetOverflow | 34F、K=999 | 圧縮後264bitで失敗 | 合格 |
| ReservedExpression | 4F + 既存同期Int Index | 予約名で失敗 | 合格 |
| ReservedFxSameType | 4F、FXに既存Int Index | 予約名で失敗 | 合格 |
| ReservedFxWrongType | 4F、FXに既存Trigger Index | 予約名で失敗 | 合格 |
| WrongTargetType | 4F、FXのF0はInt | 型不一致で失敗 | 合格 |
| WrongIsLocalType | 4F、FXのIsLocalはFloat | 型不一致で失敗 | 合格 |
| ExistingLayer | 4F、生成予定名と同じLayer | 衝突で失敗、既存Layer保持 | 合格 |
| TimingTarget | 4F、予約パスを持つ直下の子 | 衝突で失敗 | 合格 |
| DuplicateExpression | 4F + 同名F0のInt | 重複・型不一致で失敗 | 合格 |
| CustomBitWidth | 5F、F0は4bit、F1はカスタム補間 | F0の同期維持、F1の設定保持、最終20bit、4 Batch | 合格 |
| ReservedWithoutTargets | 0件、FXに既存Int Index | 予約名で失敗 | 合格 |
| Disabled | 5F、圧縮無効 | 成功、生成なし | 合格 |
| Contact | 5F、Contact Receiverの対象がF0 | F0の同期維持、4 Batch | 合格 |

`BuildContext.Successful` の期待値と、正常系での同期Parameter・Layer数・State数・Driverを比較します。各BatchでEncoderの最後がIndex Setであり、DecoderのCopyが逆向きであることを検査します。エラー系はNDMFのエラーログと `Successful == false` を確認します。各ケースで保存前後のPrefab、FX、Expression Parametersをバイト比較し、元Expression ParametersのJSONと元FXのParameter・Layer名も照合します。成功時は `LITEKIT_CASE_PASS` が21行、最後に `LITEKIT_CONTRACT_PASS 21`、終了コード0になります。

## VRChat SDK Builder の実測

| ケース | 入力 | 期待値 | 実測 |
|---|---|---|---|
| A | 3F + 3I、K=1、圧縮有効 | 48 → 24bitに圧縮してBundleを出力 | NDMFに圧縮ログ、SDKがBundleを返し、例外なし |
| B | Aに既存の同期Int `LiteKit_PC_Sync_Index` を追加 | 予約名を報告しSDK Build失敗 | 予約名をログに記録。NDMFの前処理がSDK Buildを中断し、Bundleなし |
| C | AのFXに `LiteKit Parameter Compressor [4ED03D94]` を追加 | 同名Layerを報告し、既存Layerを保持してSDK Build失敗 | Layer名をログに記録。NDMFの前処理がSDK Buildを中断し、Bundleなし。既存Layer保持 |
| D | 同期Bool 1件、圧縮有効 | 圧縮をスキップしてBundleを出力 | 対象なしのログ、SDKがBundleを返し、例外なし |

検証環境は上記と同じです。自動判定版の実行ログでA〜DのPASSと集計 `LITEKIT_SDK_PASS 4` を確認しました。SDKの公開 `IVRCSdkAvatarBuilderApi.Build(GameObject)` でローカルBundleを生成しました。認証画面を開かず、認証・Upload操作は行っていません。検証AvatarはAnimator、Descriptor、PipelineManager（Blueprint IDは空）、LiteKit Component、カスタムFX、Expression Parameters、空のExpression Menuを持つPrefabです。A〜Dすべてで元Prefab・FX・Expression Parametersのバイト比較、元Expression ParametersのJSON、元FXのLayer名が一致しました。

### 自動判定と再実行

[SDK Build用コード](../.github/verification/LiteKitSdkCheck.cs.txt)を一時プロジェクトの `Assets/Editor/LiteKitSdkCheck.cs` にコピーします。上のバッチコマンドで `-executeMethod` を `LiteKitSdkCheck.Run` に、ログファイル名を `sdk-contract.log` に変更します。終了コードを検査し、次のコマンドで結果を確認できます。

```powershell
Select-String -Path (Join-Path $project 'sdk-contract.log') -Pattern '^LITEKIT_SDK_PASS '
```

ケースごとのログ購読で、そのケースのSDK Build中に出た診断を収集します。Aは実在するBundleとLiteKitの `48 → 24 bits (-24)`、Dは実在するBundleと対象なしの診断を検査します。B・Cは、Bundleを返さずSDKの `BuilderException` が発生したこと、LiteKit例外に該当する名前と衝突理由があること、SDKがNDMFの前処理コールバック失敗を報告したことを検査します。日英両方の診断文を受け付けます。Cは元FXの既存Layerが残っていることも確認します。

各ケースが合格すると `LITEKIT_SDK_PASS A` のように出力されます。全4件に合格し、後片付けも完了した場合のみ `LITEKIT_SDK_PASS 4` と終了コード0になります。期待値との差異、例外、95秒のタイムアウトは失敗として診断し、終了コード1を返します。ログ購読を解除し、作成したオブジェクトとケース固有の一時Assetを片付けます。

一時コピーでBの期待診断だけを意図的に変え、SDKが失敗しても衝突診断が一致しなければテストが失敗し、終了コード1になることも確認しました。

## 残る検証

VRChatクライアント間でのChannelとIndexの到着順・取りこぼし、PC / Android / iOS間の同期、VRCFury等との併用順、宣言下限のSDK 3.7.0・NDMF 1.6.0でのビルドは未検証です。
