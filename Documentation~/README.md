# VRC Avatar LiteKit Plus

メニュー: `Tools > VRC Avatar LiteKit Plus > Open LiteKit Window`

VRC Avatar LiteKit Plus は、VRChatアバターの軽量化診断、Performance Rank目標への到達支援、Quest / Android / iOS向け変換支援を目的とした、非破壊方針のUnity Editor拡張です。

現時点では初期開発版であり、本格的なTexture Atlas化、UV変更、Mesh結合、ポリゴン削減、Material変換は実装していません。v0.1系では診断、一覧化、Dry Runレポートを中心に扱います。

## 初期方針

- Dry Runとログ出力を優先します。
- 元Avatar / Prefab / Material / Texture / Meshを直接変更しません。
- Performance Rankは推定であり、VRChat SDK BuilderのValidation結果と完全一致しない可能性があります。
- Mobile Shader判定などは簡易判定から始め、完全判定ではないことをUIやレポートで明示します。
- NDMF連携は将来機能としてPlaceholderに留めています。
- 破壊的操作は、将来実装する場合でも明示的な確認なしに実行しない設計にします。
