# VRC Avatar LiteKit Plus

`VRC Avatar LiteKit Plus` は、VRChatアバターの軽量化診断、Performance Rankの目標確認、Mobile / Quest / Android / iOS向け変換支援を目的とした、非破壊方針のUnity Editor拡張です。

このリポジトリは `vrc-avatar-toolkit-plus` の姉妹プロジェクトです。診断、可視化、Dry Runレポートに加え、v0.2からNDMF Build clone上で動くParameter Compressorを提供します。

## Parameter Compressor（v0.2）

Avatar Rootの `VRC Avatar LiteKit Plus/Parameter Compressor` Componentを追加し、通常どおりBuildすると、同期Float / Int Parameterを時分割多重化します。Bool、VRChat Built-in、Face Tracking、既知の他ツール内部Parameterは対象外です。

元Parameterは削除せず、Build clone上で `networkSynced` だけをfalseへ切り替えるため、名前、型、`saved`、`defaultValue`を維持します。同期チャンネル数を増やすと遅延が減る代わりにbit使用量が増えます。詳しくは[Parameter Compression設計](docs/PARAMETER_COMPRESSION.md)を参照してください。

## v0.1系でできること

- ObjectField または現在のUnity選択から Avatar Root を指定できます。
- 公開されているVRChat Performance Rank閾値を参考に、Dry Runの推定診断を行います。
- PC / Mobile の目標プラットフォームと目標ランクとの差分を確認できます。
- Renderer、Material、Texture、Mobile向け警告を一覧表示します。
- Markdown / JSON形式の診断レポートをコピーできます。
- 非アクティブなGameObjectや無効Componentも含めて診断する方針です。

## まだできないこと

- 元Prefab、元Material、元Texture、元Meshを直接書き換える自動最適化。
- Texture Max Size変更、Texture Atlas生成、UV変更。
- Mesh結合、ポリゴン削減、Material統合。
- Poiyomi / lilToon などの完全なShader変換。
- Unsupported Componentの自動削除または無効化。
- VRChat SDK Build / Upload / MultiPlatform Upload API の呼び出し。

## 非破壊・Dry Run方針

このツールは、元アセットを守ることを最優先にします。

- 元Avatar、Prefab、Material、Texture、Meshを直接変更しません。
- まず診断、提案、ログ、Before / Afterの見積もりを表示します。
- 破壊的または復元が難しい操作は、自動フローに含めません。
- 将来の変換機能でも、生成物を別Assetとして作成する方針です。
- 削除や上書きが必要になる可能性のある機能は、明示的なユーザー確認なしに実装・実行しません。

## Performance Rankについて

LiteKitのPerformance Rankは、VRChat公式の公開閾値に基づく推定です。VRChat SDK BuilderのValidation結果と完全に一致するとは限りません。

そのため、Unity上のUIやレポートでは「推定」であることを明示し、最終確認はVRChat SDK Builder側のValidation結果も確認する前提で扱ってください。

## VPM / UPMでの導入

このリポジトリは、リポジトリ直下をVPM / UPM package rootとして扱います。`package.json`、`Editor/`、`Runtime/`、`Documentation~/` はリポジトリ直下に配置します。

VPM配布では、GitHubの `Source code (zip)` ではなく、Releaseに手動添付したVPM用Release asset ZIPを参照する方針です。ZIPを作成する場合も、package rootの中身がZIP直下に入るように手動で確認してください。

## 開発者向けドキュメント

- 作業方針と安全ルール: [`AGENTS.md`](AGENTS.md)
- 設計メモ: [`docs/DESIGN.md`](docs/DESIGN.md)
- 安全方針: [`docs/SAFETY_POLICY.md`](docs/SAFETY_POLICY.md)
- ロードマップ: [`docs/ROADMAP.md`](docs/ROADMAP.md)
- Mobile変換メモ: [`docs/MOBILE_CONVERSION_NOTES.md`](docs/MOBILE_CONVERSION_NOTES.md)
- Texture最適化メモ: [`docs/TEXTURE_OPTIMIZATION_NOTES.md`](docs/TEXTURE_OPTIMIZATION_NOTES.md)
- Issue草案: [`docs/ISSUE_DRAFTS.md`](docs/ISSUE_DRAFTS.md)
- Parameter Compression: [`docs/PARAMETER_COMPRESSION.md`](docs/PARAMETER_COMPRESSION.md)

## 将来の連携候補

Parameter CompressorはVRChat SDK3 - AvatarsとNDMFを必須とします。Modular Avatar、VRCFury、MenuBuilderは依存関係に含めず、設定を自動変更しません。
