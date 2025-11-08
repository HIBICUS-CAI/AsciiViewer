namespace AsciiViewer;

internal static class Program
{
    private static void Main(string[] args)
    {
#if DEBUG
        args.ToList().ForEach(Console.WriteLine);
        // リモートデバッグ時アタッチするまで待機用
        if (args.Length >= 1 && int.TryParse(args[0], out int loopCount))
        {
            for (int i = 0; i < loopCount; i++)
            {
                Thread.Sleep(1000);
            }
        }
#endif

        // 引数チェック
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: AsciiViewer <image-path>");
            Console.WriteLine("Example: AsciiViewer image.png");
            return;
        }

        string imagePath = args[0];
        
        // 画像ファイルの存在確認
        if (!File.Exists(imagePath))
        {
            Console.Error.WriteLine($"Error: Image file not found: {imagePath}");
            return;
        }

        try
        {
            // ASCII画像ビューワーを実行
            ViewImageAsAscii(imagePath);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
#if DEBUG
            Console.Error.WriteLine(ex.StackTrace);
#endif
        }
    }

    private static void ViewImageAsAscii(string imagePath)
    {
        // 設定
        const string fontFamily = "Consolas";
        const float fontSize = 12;
        const int charWidth = 8;
        const int charHeight = 16;

        // コンソールサイズを取得
        int consoleWidth = Console.WindowWidth - 1; // 右端の改行を避ける
        int consoleHeight = Console.WindowHeight - 2; // 余裕を持たせる

        // 画像を読み込み
        using var imageProbe = new ImageProbe(imagePath);
        
        // 画像サイズに基づいてASCIIグリッドサイズを計算
        int gridWidth = Math.Min(consoleWidth, imageProbe.Width / charWidth);
        int gridHeight = Math.Min(consoleHeight, imageProbe.Height / charHeight);

        Console.WriteLine($"Image: {imagePath}");
        Console.WriteLine($"Size: {imageProbe.Width}x{imageProbe.Height}");
        Console.WriteLine($"ASCII Grid: {gridWidth}x{gridHeight}");
        Console.WriteLine();

        // フォントプローブとASCIIサーフェスを作成
        using var fontProbe = new FontProbe(fontFamily, fontSize, charWidth, charHeight);
        var asciiSurface = new AsciiSurface(gridWidth, gridHeight, fontProbe);

        // 画像をASCIIに変換
        asciiSurface.ConvertFromImage(imageProbe, charWidth, charHeight);

        // コンソールレンダラーで描画
        var renderer = new ConsoleRenderer();
        renderer.RenderLine(asciiSurface);
    }
}
