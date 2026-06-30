# AGENTS.md

このリポジトリは `VRC Avatar LiteKit Plus` の Unity / VRChat 向け VPM パッケージです。

## 基本方針

- 日本語コメントは許可します。既存の文体に合わせてください。
- public API、package layout、namespace、ファイル構造を勝手に大きく変更しないでください。
- 大きな機能は小さい PR / Issue に分けてください。
- `package.json` の `version` は、明示的な指示がある場合のみ更新してください。

## 安全性

- 破壊的処理を実装しないでください。
- 元アバター、元 Prefab、元 Material、元 Texture、元 Mesh の削除は禁止です。
- 自動変換系の処理を追加する場合は、Dry Run とログ出力を必須にしてください。
- 破壊的変更を伴う可能性がある処理は、将来実装する場合でも明示的な確認なしに実行しない設計にしてください。

## 外部 API / 依存

- NDMF / Modular Avatar / VRChat SDK の API を使う場合は、既存コードと公式ドキュメントを確認してから実装してください。
- 不明な外部依存を勝手に追加しないでください。
- 初期段階では NDMF / MA / VRChat SDK への直接依存を避け、必要に応じて Placeholder に留めてください。
