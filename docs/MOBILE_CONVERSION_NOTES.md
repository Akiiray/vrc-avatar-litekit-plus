# Mobile変換メモ

Mobile変換支援は、まず推奨事項とDry Runレポートから開始します。v0.1系では自動変換を行わず、警告、診断、見積もりに限定します。

## 対象

- Quest / Android向けVRChatアバター。
- 必要に応じたAndroid handheld向け制約の整理。
- 将来のVRChatワークフローでiOS対応が必要になった場合の計画。

## 初期スコープ

- Texture sizeの推奨事項。
- Material数とTexture memoryの概要。
- Performance Rankの目標計画。
- Mobile Shaderの簡易判定。
- Mobile向け警告の一覧化。

## 初期スコープ外

- Shaderの自動変換。
- Materialの自動書き換え。
- Mesh simplification。
- Unsupported Componentの削除または無効化。
- Build Targetの自動切り替え。
- VRChat SDK Build APIやUpload APIの呼び出し。

## 注意点

Mobile Shader判定は、当面 `shader.name.StartsWith("VRChat/Mobile/")` のような簡易判定で構いません。ただし、完全判定ではないことをUI、ログ、レポートで明示します。

Quest / Android / iOS向けのBaked Avatar ExporterやPlatform Exporterは将来機能です。VPM / Release ZIPと、将来のAvatar Export ZIPを混同しないでください。
