## 変更概要

-

## 安全確認

- [ ] 元 Material / Texture / Mesh / Prefab を書き換えていない
- [ ] `AssetDatabase.DeleteAsset` を使っていない
- [ ] 自動修正を勝手に追加していない
- [ ] Dry Run / ログ出力方針を壊していない
- [ ] 外部依存をユーザー確認なしに追加していない
- [ ] `package.json` の version は、明示指示がある場合のみ変更している

## レビュー Checklist

### 言語・UI

- [ ] ユーザー向け UI 文言は日本語か
- [ ] HelpBox、ボタン、ログ、レポート文言が日本語になっているか
- [ ] Performance Rank など固有名詞は必要以上に翻訳していないか
- [ ] 「推定」であることを明示しているか

### Package 構造

- [ ] `package.json` はリポジトリ直下に 1 つだけか
- [ ] `Packages/` ディレクトリを復活させていないか
- [ ] package root が入れ子になっていないか
- [ ] `Editor/` と `Runtime/` はリポジトリ直下か
- [ ] asmdef 名が重複していないか

### namespace

- [ ] UnityEngine 型名と衝突する namespace を作っていないか
- [ ] `Editor.Texture` のような namespace がないか
- [ ] `UnityEngine.Texture` 参照は曖昧でないか
- [ ] `jp.unisakistudio` 系が残っていないか

### 依存関係

- [ ] VRChat SDK Build API を勝手に呼んでいないか
- [ ] NDMF 依存を勝手に追加していないか
- [ ] 外部依存を勝手に追加していないか
- [ ] SDK 内部 API へ Reflection でアクセスしていないか

### Release / VPM

- [ ] version 変更が必要な変更か確認したか
- [ ] 公開済み version の中身差し替えを前提にしていないか
- [ ] zip 生成スクリプトや GitHub Actions を追加していないか
- [ ] VPM URL に Source code zip を使っていないか
- [ ] Release asset ZIP を参照する方針になっているか

## Testing

-

## 未確認事項 / Notes

-
