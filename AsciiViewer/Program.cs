namespace AsciiViewer
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
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
            }

            return 0;
        }
    }
}
