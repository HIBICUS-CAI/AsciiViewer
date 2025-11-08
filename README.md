# AsciiViewer

以下の問題を解決するための~~ツール~~おもちゃである。

- SSH利用などのシーンでもコンソールで画像を見たい
- ディスプレイが付かなかったり、GUIがない環境でも、グラフィックスAPIの描画結果をリアルタイムで見たい

## 使い方

### ビルド

```bash
dotnet build
```

### 実行

```bash
dotnet run --project AsciiViewer <画像ファイルパス>
```

例：
```bash
dotnet run --project AsciiViewer image.png
```

## アーキテクチャ

プロジェクトは以下のモジュールで構成されています：

1. **TexelDistribution** - 指定領域のカラー特徴を検出
2. **FontProbe** - フォントを描画し、文字の密度を評価
3. **ImageProbe** - 画像をサンプリングし、輝度を評価
4. **AsciiSurface** - 画像をASCII文字に変換
5. **ConsoleRenderer** - コンソールへの描画処理
6. **Program** - メインアプリケーション

## 依存関係

- .NET 8.0
- SkiaSharp 3.119.1
- SkiaSharp.NativeAssets.Linux 3.119.1

## 実装済み機能

- [x] 画像をASCII文字に変換
- [x] コンソールサイズに自動調整
- [x] 文字密度マッチングアルゴリズム
- [x] クロスプラットフォーム対応

## 今後の拡張予定

- [ ] ディザリング表現
- [ ] 画像シーケンス/アニメーション対応
- [ ] カスタムフォント実装
