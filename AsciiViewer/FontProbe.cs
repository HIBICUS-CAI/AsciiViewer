using SkiaSharp;

namespace AsciiViewer;

/// <summary>
/// フォントを描画し、TexelDistributionを使って特徴を評価するモジュール
/// </summary>
public class FontProbe
{
    private readonly string _fontFamily;
    private readonly float _fontSize;
    private readonly int _width;
    private readonly int _height;
    private readonly SKFont _font;
    private readonly SKPaint _paint;

    public FontProbe(string fontFamily, float fontSize, int width, int height)
    {
        _fontFamily = fontFamily;
        _fontSize = fontSize;
        _width = width;
        _height = height;

        _font = new SKFont(SKTypeface.FromFamilyName(fontFamily), fontSize);
        _paint = new SKPaint
        {
            IsAntialias = true
        };
    }

    /// <summary>
    /// 指定文字を描画してピクセルデータを取得する
    /// </summary>
    public byte[] RenderChar(char c)
    {
        using var surface = SKSurface.Create(new SKImageInfo(_width, _height, SKColorType.Rgba8888));
        using var canvas = surface.Canvas;

        // 背景を黒で塗りつぶす
        _paint.Color = SKColors.Black;
        canvas.DrawRect(0, 0, _width, _height, _paint);

        // 文字を白で描画
        _paint.Color = SKColors.White;
        canvas.DrawText(c.ToString(), 0, _fontSize, _font, _paint);

        // ピクセルデータを取得
        using var pixmap = surface.PeekPixels();
        var pixels = pixmap.GetPixelSpan();
        return pixels.ToArray();
    }

    /// <summary>
    /// 文字を描画して特徴を評価する
    /// </summary>
    public float EvaluateChar(char c)
    {
        var pixels = RenderChar(c);
        var distribution = new TexelDistribution(pixels, _width, _height, 4); // RGBA = 4 bytes per pixel
        
        // 白い部分の割合を返す（文字の密度を表す）
        return distribution.CalculateWhiteRatio();
    }

    public void Dispose()
    {
        _font?.Dispose();
        _paint?.Dispose();
    }
}
