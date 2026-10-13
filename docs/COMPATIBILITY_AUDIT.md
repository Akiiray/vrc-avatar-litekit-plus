# 公開互換性・単独導入監査

本監査はPR前main `667030de52499de1d93209253adc9ef1f5f0a7dc`とPR #11を比較します。Toolkit #25 / VPM #3も責務・依存・リンクを照合しました。Unity Editor / VCC / ALCOMによる導入・実動作は未検証です。

## Public Surface Compatibility

| Surface | Before | After | 分類・互換性 |
| --- | --- | --- | --- |
| AvatarContext.Warnings | IReadOnlyList<string> | 同じ型・旧英語文面 | 既存契約維持 |
| AvatarContext public ctor | 最終引数IReadOnlyList<string> | 全引数の型・名前・順序を維持 | 既存契約維持 |
| AvatarContext.Diagnostics | なし | IReadOnlyList<DiagnosticMessage> | 意図した互換追加。internal ctorで設定、旧ctorでは空 |
| DiagnosticMessage | なし | Code / Argument / Severity / Category / Path | 構造化診断を受け取るための互換追加 |
| ScannerWarnings JSON | string[] | string[]、旧文面 | 既存契約維持 |
| MobileWarnings JSON | Severity / Category / Path / Message | 同じ4文字列フィールド・旧英語Message | 既存契約維持 |
| Mobile Code / Argument | なし | NonSerialized | 表示用追加。JSON非公開 |
| Report ScannerDiagnostics | なし | NonSerialized | UI / Markdown用追加。JSON非公開 |
| AnalyzedAvatar / AnalysisCompleted / IsStale | なし | NonSerialized | 実行時追加。JSON非公開 |
| Report.Matches | なし | 対象・条件・stale判定 | 意図した互換追加 |
| UsesNumericUpperBound | なし | NonSerialized | 表示判断用追加。JSON非公開 |
| PerformanceMetricKey / 各既存enum | 既存値 | 定義・数値・特殊診断のKeyを維持 | 既存契約維持 |
| Notes | public / serialized string | 未設定時だけ既定文、設定済みはそのまま出力 | 呼出元データ保持 |
| ToMarkdown / ToJson / FormatBytes | 既存public signature | 全signature維持 | 既存契約維持。Markdown見出し・翻訳は表示改善 |
| その他既存public型・method・property・field・ctor | PR前の宣言 | 削除・rename・型／引数変更なし | 全C#の宣言比較で確認 |
| NdmfIntegrationPlaceholder.Status | 将来NDMF全般の説明 | Compressor以外の将来最適化の説明 | 意図した説明文の意味更新。型・名前は維持 |

現行の追加publicデータには内部用途のものがありますが、既にPR内でDiagnostics APIに使う型をinternalにして不整合を作らないよう維持します。Localization / formatter helper / 新constructorはinternalまたはprivateです。既存公開面をinternalへ移す変更は行いません。

## JSONと表示の境界

serialized公開フィールド名・型、nestedデータ型、array element型をPR前mainと比較し、既存schemaを維持しています。Unity JsonUtilityの実行確認は別途必要です。

- live report: 構造化診断をLocalization経由でUI / Markdownへ表示。
- JSON復元・旧公開フィールドだけのreport: ScannerDiagnosticsがない場合はScannerWarnings、Mobile Codeがない場合は旧Messageへfallback。旧英語文面を翻訳のために解析しません。
- Scanner警告とMobile警告は別見出し。
- 数値上限は明示的な内部情報を優先。復元後は閾値定義とtyped NumericValue / EstimatedRankの整合性で表示方向を補います。表示文章から型を推測しません。Trianglesを再利用するRead/Write特殊診断はvalue=0 / rank=VeryPoorなので数値閾値と不整合となり、上限表示を付けません。
- Notesがnull / 空ならLocalized既定文、存在すれば完全に保持。
- minimal reportの未設定collection / statsを許容し、警告とNotesだけでもMarkdownを生成。

## Localization

Window、Inspector、HelpBox、button、label、Preview説明、Markdown固定文を検索しました。InspectorのL(japanese, english)とFormatterの言語別固定文をLiteKitTextKey / LiteKitLocalizationへ集約しました。identifier、製品名、enum名、Shader名、内部ログ、互換用旧英語文面は翻訳キー化の対象外です。

## 単独導入matrix（静的判定）

| 構成 | 静的判定 | 必須依存 | 問題・未検証 |
| --- | --- | --- | --- |
| Toolkitのみ | LiteKitへのC# / asmdef / manifest必須参照なし | VRChat Avatars SDK >=3.7.0 | Optionalツールは型・Prefab検出で扱う。実導入・Reflection互換性は未検証 |
| LiteKitのみ | Toolkitへの必須参照なし。依存metadataを修正 | SDK 3.7.0、NDMF 1.5.0（既存Version指定維持） | 公開0.2.0の依存欄は後述。実解決・コンパイル未検証 |
| Toolkit + LiteKit | ID / assembly名 / namespace / MenuItem衝突、兄弟dependency cycleなし | 上記の共通SDKとNDMF | SDK制約の共通部分は3.7.0。新しいSDKへの制約拡張は今回行わず、Toolkit PreflightのSDK 3.10.5基準との実環境互換性は別検証 |

Editor / Runtime参照を確認しました。NDMFの直接利用はLiteKit Compressorにあり、ToolkitからNDMFを要求しません。両方が存在するときだけ成立するcompile-time参照はありません。兄弟Packageを検出条件にするOptional integrationも見つかりませんでした。文書のGitHubリンクは依存関係ではありません。

## VPM dependency metadata

[VRChat公式VPM package仕様](https://vcc.docs.vrchat.com/vpm/packages/)は、VPM依存をvpmDependenciesに列挙すると明記しています。LiteKitのasmdefはnadena.dev.ndmf / VRC.SDK3A / VRC.SDKBaseを直接参照するため、source package.jsonのdependenciesをvpmDependenciesへ修正しました。依存名・Version指定とpackage versionは変えていません。UPM直接導入ではVPM依存を別途導入してください。VCC / ALCOM側に依存を提供するVPMリポジトリが登録されていることも必要です。

VPMカタログの公開済みLiteKit 0.2.0にもdependencies欄のみがあり、vpmDependenciesがありません。カタログ上の依存解決漏れは次回patch releaseで修正すべき配布問題です。今回assetをダウンロードして中身を検証したわけではありません。既存0.2.0のasset / index / hashは差し替えず、新Package release → 実asset → SHA256 → 新Versionのindex追加で扱います。

Toolkit 0.2.18のsource / indexはSDK依存の意味が一致しています。LiteKit修正sourceと公開0.2.0 catalogの差は、次回Releaseで反映する未公開修正です。

## 検証と再現手順

既存test infrastructureはありません。Packageへtest frameworkや新dependencyは追加していません。作業用.NET 8ハーネスで実際のContext / Report / Evaluator / Localization / Formatterソースをコンパイルし、354件のassertionが通過しました。Unity型とEditorPrefsをstub化し、field-only serializationはSystem.Text.Jsonで模擬しています。これはUnityコンパイルや本物のJsonUtility round-tripの検証ではありません。

静的検証: git diff --check、全C#構文、全public宣言、Localization key不足・重複、既存serializedフィールド、全enum定義、Compressor内部不変性、兄弟参照・依存・メニュー、文書リンクを確認しました。

実Unityでは次を再現してください。

1. 旧引数でnew AvatarContextを作り、WarningsをIReadOnlyList<string>として受け取る。Diagnosticsは空であること。
2. Missing Material / Missing Scriptを診断し、旧Warningsと翻訳済み表示・Severityの分離を確認。
3. Copy JSONをJsonUtility.FromJson<LiteKitReport>で復元し、両言語のToMarkdownへ渡す。旧Scanner文字列とMobile Messageが消えないこと。
4. Notesへ任意文字列を設定し、live / JSON復元後双方で完全保持。null / 空では言語別既定文。
5. Triangles / Texture Memory / Material Slotsは≤、Particle Trails / CollisionとRead/Write条件は≤なし。PC / Mobile、Good / Poor、live / 復元後を比較。
6. Avatar / Platform / Rank変更時のstale、言語 / Foldout変更で維持されることを確認。
7. Toolkit単独、LiteKit単独、両方の新規VCC / ALCOM導入で依存解決とUnityコンパイルを確認。NDMF配布リポジトリ登録の有無も確認。

merge、Version、CHANGELOG Release entry、tag、Release、asset、hash、VPM indexは変更していません。
