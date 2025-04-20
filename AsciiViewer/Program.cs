using Logger;

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

        using (new LogNest<Viewer>("A"))
        {
            Viewer.Logger.Log("hello world 0");
            using (new LogNest<Viewer>("B"))
            {
                Viewer.Logger.Log("hello world 1");
            }

            using (new LogNest<Viewer>("C"))
            {
                Viewer.Logger.Log("hello world 2");
                using (new LogNest<Viewer>("D"))
                {
                    Viewer.Logger.Log("hello world 3");
                }

                Viewer.Logger.Log("hello world 4");
            }

            Viewer.Logger.Log("hello world 5");
        }

        using var _ = new LogNest<Viewer>("hogehoge");
        Viewer.Logger.Log("hello world 0");
        {
            using var nest1 = new LogNest<Viewer>("fugafuga");
            Viewer.Logger.Log("hello world 1");
            {
                using var nest2 = new LogNest<Viewer>("piyopiyo");
                Viewer.Logger.Log("hello world 2");
            }
            Viewer.Logger.Log("hello world 3");
        }
        Viewer.Logger.Log("hello world 4");

        Viewer.Logger.Log("hello world 0");
        using var nest3 = new LogNest<Viewer>("hoge");
        Viewer.Logger.Log("hello world 1");
        using var nest4 = new LogNest<Viewer>("fuga");
        Viewer.Logger.Log("hello world 2");
        using var nest5 = new LogNest<Viewer>("piyo");
        Viewer.Logger.Log("hello world 3");
        Viewer.Logger.Log("hello world 4");
        Viewer.Logger.Log("hello world 5");
        Viewer.Logger.Log("hello world 6");

        nest3.Logger.Log("hello world 7");

        var tempLogger = new NestableLogger { Category = "Temp" };
        using (new LogNest(tempLogger, "temp hoge"))
        {
            tempLogger.Log("hello world 8");
        }
    }
}

internal abstract class Viewer : ILoggerInstance
{
    public static NestableLogger Logger { get; } = new() { Category = nameof(Viewer) };

    public static StandardLogger Get()
    {
        return Logger;
    }
}
