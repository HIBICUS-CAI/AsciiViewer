namespace AsciiViewer;

/// <summary>
/// 指定領域のカラー特徴を検出するモジュール
/// </summary>
public class TexelDistribution
{
    private readonly byte[] _data;
    private readonly int _width;
    private readonly int _height;
    private readonly int _bytesPerPixel;

    public TexelDistribution(byte[] data, int width, int height, int bytesPerPixel)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _width = width;
        _height = height;
        _bytesPerPixel = bytesPerPixel;

        if (data.Length != width * height * bytesPerPixel)
        {
            throw new ArgumentException("Data length does not match width * height * bytesPerPixel");
        }
    }

    /// <summary>
    /// 指定領域の平均輝度を計算する
    /// </summary>
    public float CalculateAverageLuminance()
    {
        if (_data.Length == 0) return 0f;

        long sum = 0;
        int pixelCount = _width * _height;

        for (int i = 0; i < pixelCount; i++)
        {
            int offset = i * _bytesPerPixel;
            
            // RGB or RGBA format assumed
            byte r = _data[offset];
            byte g = _data[offset + 1];
            byte b = _data[offset + 2];

            // Calculate luminance using standard formula
            // Y = 0.299R + 0.587G + 0.114B
            int luminance = (int)(0.299f * r + 0.587f * g + 0.114f * b);
            sum += luminance;
        }

        return (float)sum / pixelCount;
    }

    /// <summary>
    /// 文字描画用：白い部分の割合を計算する（黒を背景色として扱う）
    /// </summary>
    public float CalculateWhiteRatio(byte threshold = 128)
    {
        if (_data.Length == 0) return 0f;

        int whitePixelCount = 0;
        int pixelCount = _width * _height;

        for (int i = 0; i < pixelCount; i++)
        {
            int offset = i * _bytesPerPixel;

            byte r = _data[offset];
            byte g = _data[offset + 1];
            byte b = _data[offset + 2];

            // Calculate luminance
            int luminance = (int)(0.299f * r + 0.587f * g + 0.114f * b);

            if (luminance > threshold)
            {
                whitePixelCount++;
            }
        }

        return (float)whitePixelCount / pixelCount;
    }
}
