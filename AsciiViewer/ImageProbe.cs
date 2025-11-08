using SkiaSharp;

namespace AsciiViewer;

/// <summary>
/// 画像をサンプリングし、TexelDistributionを使って特徴を評価するモジュール
/// </summary>
public class ImageProbe : IDisposable
{
    private readonly SKBitmap _bitmap;

    public ImageProbe(string imagePath)
    {
        _bitmap = SKBitmap.Decode(imagePath);
        if (_bitmap == null)
        {
            throw new ArgumentException($"Failed to load image: {imagePath}");
        }
    }

    public ImageProbe(SKBitmap bitmap)
    {
        _bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
    }

    public int Width => _bitmap.Width;
    public int Height => _bitmap.Height;

    /// <summary>
    /// 指定領域をサンプリングしてピクセルデータを取得する
    /// </summary>
    public byte[] SampleRegion(int x, int y, int width, int height)
    {
        // 範囲チェック
        if (x < 0 || y < 0 || x + width > _bitmap.Width || y + height > _bitmap.Height)
        {
            throw new ArgumentOutOfRangeException("Sample region is out of bounds");
        }

        // RGBA形式のバッファを作成
        var pixels = new byte[width * height * 4];
        
        // 指定領域のピクセルを読み取る
        int pixelIndex = 0;
        for (int dy = 0; dy < height; dy++)
        {
            for (int dx = 0; dx < width; dx++)
            {
                var color = _bitmap.GetPixel(x + dx, y + dy);
                pixels[pixelIndex++] = color.Red;
                pixels[pixelIndex++] = color.Green;
                pixels[pixelIndex++] = color.Blue;
                pixels[pixelIndex++] = color.Alpha;
            }
        }

        return pixels;
    }

    /// <summary>
    /// 指定領域を評価する（平均輝度を返す）
    /// </summary>
    public float EvaluateRegion(int x, int y, int width, int height)
    {
        var pixels = SampleRegion(x, y, width, height);
        var distribution = new TexelDistribution(pixels, width, height, 4); // RGBA = 4 bytes per pixel
        
        // 平均輝度を返す
        return distribution.CalculateAverageLuminance();
    }

    public void Dispose()
    {
        _bitmap?.Dispose();
    }
}
