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

        Console.WriteLine("Hello, World!");
    }
}
