using SkiaSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AsciiViewer
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            args.ToList().ForEach(Console.WriteLine);

            {// コンソール関連
                // サイズ取得、できれば設定
                // 設定は無理らしい、WindowsTerminalを使用する想定なので設定ファイルにアクセスしたり自前で記入してもおう
                // また、その場合バッファを大きくしても横スクロールができないので、常に一致させよう
                // ちなみに、Ctrl+Plus/Ctrl+Minusで拡大縮小できる、その値が反映できるのでDebugには便利
                Console.WriteLine($"Window: {Console.WindowWidth} * {Console.WindowHeight}");
                Console.WriteLine($"Buffer: {Console.BufferWidth} * {Console.BufferHeight}");

                // bgとfgの色設定
                // Console.ForegroundColorの色が少なさすぎ
                // https://stackoverflow.com/a/33206814
                for (var r = 0; r < 255; r += 4)
                {
                    for (var g = 0; g < 255; g += 4)
                    {
                        for (var b = 0; b < 255; b += 4)
                        {
                            Console.Write($"\x1b[48;2;{r};{g};{b};38;2;{255 - r};{255 - g};{255 - b}m*");
                        }
                        Console.WriteLine("\x1b[49;39m");
                    }
                }

                // フォントの情報を取得
                // 今後SSHで接続した場合、フォントの情報を取得できないので、あらかじめ設定するしかなさそう
            }

            {// 表現関連
                // ascii文字を指定フォントとサイズで描画
                const char character = 'A';
                const string fontName = "Consolas";
                const float fontSize = 48;
                const string outputPath = "./output.bmp";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
#pragma warning disable CA1416  // Windows-only API、本番は構成を分けた方が良い
                    var font = new Font(fontName, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                    SizeF textSize;
                    using (var tempBitmap = new Bitmap(1, 1))
                    using (var tempGraphics = Graphics.FromImage(tempBitmap))
                    {
                        textSize = tempGraphics.MeasureString(character.ToString(), font);
                    }

                    using var bitmap = new Bitmap((int)Math.Ceiling(textSize.Width), (int)Math.Ceiling(textSize.Height));
                    using var graphics = Graphics.FromImage(bitmap);
                    graphics.Clear(Color.White);
                    using Brush brush = new SolidBrush(Color.Black);
                    graphics.DrawString(character.ToString(), font, brush, 0, 0);
                    bitmap.Save(outputPath, ImageFormat.Bmp);
                    var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                    var bytes = new byte[Math.Abs(data.Stride) * bitmap.Height];
                    var ptr = data.Scan0;
                    Marshal.Copy(ptr, bytes, 0, bytes.Length);
                    var pixelContent = bytes.AsSpan();
#pragma warning restore CA1416
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    using var skSurface = SKSurface.Create(new SKImageInfo(100, 100));
                    using var skCanvas = skSurface.Canvas;
                    using var skPaint = new SKPaint();
                    skPaint.IsAntialias = true;
                    skPaint.Color = SKColors.White;
                    skCanvas.DrawRect(0, 0, 100, 100, skPaint);
                    skPaint.Color = SKColors.Black;
                    var skFont = new SKFont(SKTypeface.FromFamilyName(fontName), fontSize);
                    skCanvas.DrawText(character.ToString(), 20, 60, SKTextAlign.Center, skFont, skPaint);
                    // 最終目的がファイルを保存ではないけど、BMPをサポートしないのが古いのが流石にあほらしい
                    const SKEncodedImageFormat format = SKEncodedImageFormat.Webp;
                    const int quality = 100;
                    var pixelContent = skSurface.PeekPixels().GetPixelSpan();
                    var bytes = skSurface.Snapshot().Encode(format, quality).ToArray();
                    using var fs = new FileStream($"{outputPath}.webp", FileMode.OpenOrCreate);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }

            return 0;
        }
    }
}
