using Logger;
using Logger.Util;

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

        using (new LogNestObject("A"))
        {
            Viewer.Logger.Log("hello world 0");
            using (new LogNestObject("B"))
            {
                Viewer.Logger.Log("hello world 1");
            }

            using (new LogNestObject("C"))
            {
                Viewer.Logger.Log("hello world 2");
                using (new LogNestObject("D"))
                {
                    Viewer.Logger.Log("hello world 3");
                }

                Viewer.Logger.Log("hello world 4");
            }

            Viewer.Logger.Log("hello world 5");
        }

        using var _ = new LogNestObject("hogehoge");
        Viewer.Logger.Log("hello world 0");
        {
            using var nest1 = new LogNestObject("fugafuga");
            Viewer.Logger.Log("hello world 1");
            {
                using var nest2 = new LogNestObject("piyopiyo");
                Viewer.Logger.Log("hello world 2");
            }
            Viewer.Logger.Log("hello world 3");
        }
        Viewer.Logger.Log("hello world 4");

        Viewer.Logger.Log("hello world 0");
        using var nest3 = new LogNestObject("hoge");
        Viewer.Logger.Log("hello world 1");
        using var nest4 = new LogNestObject("fuga");
        Viewer.Logger.Log("hello world 2");
        using var nest5 = new LogNestObject("piyo");
        Viewer.Logger.Log("hello world 3");
        Viewer.Logger.Log("hello world 4");
        Viewer.Logger.Log("hello world 5");
        Viewer.Logger.Log("hello world 6");

        nest3.Logger.Log("hello world 7");

        using var nest6 = new LogNestObject(Viewer.Logger);
        Viewer.Logger.Log("hello world 8");
    }
}

internal abstract class Viewer : ILoggerInstance
{
    public static NestableLogger Logger { get; } = new();

    public static NestableLogger Get()
    {
        return Logger;
    }
}
