# Issue草案

このファイルは、今後の作業を小さくレビューしやすい単位に分けるための草案です。各Issueは、元Avatarや元Assetを変更しないDry Run中心の作業として扱います。

## 1. Repository scaffoldとpackage metadata

### 概要

**Repository scaffold and package metadata** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

VPM / UPM package rootがリポジトリ直下にある構造を確認し、最小限のpackage metadataを安全に整備する。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。

## 2. Editor window skeleton

### 概要

**Editor window skeleton** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

Unityメニューから開けるEditor windowの骨組みを整え、未実装機能は安全なplaceholderとして表示する。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。

## 3. Avatar選択とAvatarContext

### 概要

**Avatar selection and AvatarContext** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

ObjectFieldまたは現在のUnity選択からAvatar Rootを指定し、診断用のコンテキストを構築する。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。

## 4. Texture Usage Analyzer

### 概要

**Texture Usage Analyzer** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

RendererとMaterialからTexture参照を一覧化し、未使用候補を削除ではなくレポートとして提示する。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。

## 5. UV Usage Analyzer

### 概要

**UV Usage Analyzer** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

Mesh UVを確認し、Textureの使用領域を推定するためのDry Run分析を追加する。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。

## 6. Texture Waste Report

### 概要

**Texture Waste Report** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

Texture寸法と推定使用領域を比較し、無駄が大きい可能性をレポートする。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。

## 7. Material Duplicate Finder

### 概要

**Material Duplicate Finder** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

類似または重複している可能性のあるMaterialを検出し、統合は行わず候補として提示する。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。

## 8. Performance Target Planner

### 概要

**Performance Target Planner** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

PC / Mobileの目標Performance Rankに対して、どのMetricが差分になっているかを表示する。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。

## 9. Mobile Texture Profile

### 概要

**Mobile Texture Profile** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

Quest / Android / iOS向けのTexture sizeやMemoryに関する推奨事項をDry Runで提示する。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。

## 10. NDMF Build Pass placeholder

### 概要

**NDMF Build Pass placeholder** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

NDMF連携の将来設計用placeholderを追加し、v0.1系では直接依存や本実装を行わない。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。

## 11. Safety / Dry Run logging system

### 概要

**Safety / Dry Run logging system** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

変更予定、未実装、Dry Run結果を日本語ログとレポートで分かりやすく表示する。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。

## 12. Documentation cleanup

### 概要

**Documentation cleanup** に関する次の小さな作業単位を準備します。破壊的なAvatar変更やAsset変更は導入しません。

### 目的

README、docs、Documentation~、AGENTS、Issue / PR templateを日本語中心に整理する。

### 作業内容

- 想定するpublic APIとファイル配置を確認する。
- 安全なplaceholderまたはDry Run entry pointの背後に最小実装を追加・更新する。
- 必要に応じてユーザー向けUI文言、ログ、開発者向けドキュメントを日本語で追加する。
- 元Prefab、元Material、元Texture、元Meshが変更または削除されないことを確認する。

### 受け入れ条件

- Unity 2022.3でコンパイルできる。
- 機能が独立してレビューできる。
- Dry Runまたはplaceholderであることが、ログ、UI、ドキュメントのいずれかで明確に分かる。
- ユーザーの明示承認なしに新しい外部依存を追加していない。
- `package.json` のversionを勝手に変更していない。

### 注意点・リスク

- NDMF、Modular Avatar、VRChat SDK APIを直接使う前に、公式ドキュメントと互換性を確認する。
- Texture、Material、UV、Mesh操作は範囲を誤ると破壊的になりやすいため、初期作業はレポートのみとする。
- `Packages/` ディレクトリを復活させず、package rootの入れ子を作らない。
- UnityEngine型名と衝突するnamespace、特に `Editor.Texture` のようなnamespaceを作らない。
