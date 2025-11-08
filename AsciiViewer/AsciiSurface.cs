namespace AsciiViewer;

/// <summary>
/// 画像からAscii文字に変換するモジュール
/// </summary>
public class AsciiSurface
{
    private readonly int _width;
    private readonly int _height;
    private readonly char[] _buffer;
    private readonly FontProbe _fontProbe;
    private readonly Dictionary<char, float> _charDensityMap;

    // ASCII文字セット（密度が低い順に並べる）
    private static readonly char[] DefaultCharSet = 
        " .:-=+*#%@".ToCharArray();

    public AsciiSurface(int width, int height, FontProbe fontProbe)
    {
        _width = width;
        _height = height;
        _buffer = new char[width * height];
        _fontProbe = fontProbe ?? throw new ArgumentNullException(nameof(fontProbe));
        
        // 各文字の密度を事前計算
        _charDensityMap = new Dictionary<char, float>();
        foreach (var c in DefaultCharSet)
        {
            _charDensityMap[c] = _fontProbe.EvaluateChar(c);
        }
    }

    /// <summary>
    /// 画像をASCII文字に変換する
    /// </summary>
    public void ConvertFromImage(ImageProbe imageProbe, int charWidth, int charHeight)
    {
        if (imageProbe == null)
        {
            throw new ArgumentNullException(nameof(imageProbe));
        }

        // 画像を文字グリッドに分割して変換
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                // 画像の対応する領域を計算
                int imgX = x * charWidth;
                int imgY = y * charHeight;

                // 範囲チェック
                if (imgX + charWidth > imageProbe.Width || imgY + charHeight > imageProbe.Height)
                {
                    _buffer[y * _width + x] = ' ';
                    continue;
                }

                // 領域の輝度を評価
                float luminance = imageProbe.EvaluateRegion(imgX, imgY, charWidth, charHeight);

                // 輝度を0-1の範囲に正規化（0-255 -> 0-1）
                float normalizedLuminance = luminance / 255.0f;

                // 最も近い密度の文字を選択
                char bestChar = FindBestMatchingChar(normalizedLuminance);
                _buffer[y * _width + x] = bestChar;
            }
        }
    }

    /// <summary>
    /// 指定された輝度に最も近い文字を見つける
    /// </summary>
    private char FindBestMatchingChar(float targetDensity)
    {
        char bestChar = ' ';
        float minDifference = float.MaxValue;

        foreach (var kvp in _charDensityMap)
        {
            float difference = Math.Abs(kvp.Value - targetDensity);
            if (difference < minDifference)
            {
                minDifference = difference;
                bestChar = kvp.Key;
            }
        }

        return bestChar;
    }

    /// <summary>
    /// バッファの内容を文字列として取得
    /// </summary>
    public string GetBufferAsString()
    {
        var lines = new string[_height];
        for (int y = 0; y < _height; y++)
        {
            lines[y] = new string(_buffer, y * _width, _width);
        }
        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// バッファをクリア
    /// </summary>
    public void Clear(char fillChar = ' ')
    {
        Array.Fill(_buffer, fillChar);
    }

    public int Width => _width;
    public int Height => _height;
}
