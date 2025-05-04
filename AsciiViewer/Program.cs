using System.CommandLine;
using Logger;

namespace AsciiViewer;

internal static class Program
{
    private static void Main(string[] args)
    {
        var debugWaitingOption = new Option<int>(
            aliases: ["--debug-waiting", "-w"],
            description: "wait for N seconds after start up bu before doing task.",
            getDefaultValue: () => 0
        );

        var rootCommand = new RootCommand("AsciiViewer, now it's only a playground.");
        rootCommand.AddOption(debugWaitingOption);
        rootCommand.SetHandler(_Test, debugWaitingOption);

        _ = rootCommand.Invoke(args);
    }

    private static void _Test(int debugWaitingSeconds)
    {
        if (debugWaitingSeconds > 0)
        {
            Thread.Sleep(1000 * debugWaitingSeconds);
        }

        LogPrinter.Print += PrintLogToDebugOutput;
        LogPrinter.Print += PrintLogToStdOutput;

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

        LogPrinter.Print -= PrintLogToDebugOutput;
        LogPrinter.Print -= PrintLogToStdOutput;
        return;

        void PrintLogToDebugOutput(object? _, LogPrintArgs logPrintArgs)
        {
            var logInfo = logPrintArgs.LogElement;
            bool isNestable = logPrintArgs.LoggerFeature.HasFlag(LoggerFeature.Nestable);
            System.Diagnostics.Debug.WriteLine($"[{logInfo.Category}], {logInfo.DateTime.TimeOfDay}");
            System.Diagnostics.Debug.WriteLine($"\t{string.Join('/', isNestable ? logInfo.ProcessNestFrame ?? [] : [])}, {logInfo.Message}");
        }

        void PrintLogToStdOutput(object? _, LogPrintArgs logPrintArgs)
        {
            var logInfo = logPrintArgs.LogElement;
            bool isNestable = logPrintArgs.LoggerFeature.HasFlag(LoggerFeature.Nestable);
            Console.WriteLine($"[{logInfo.Category}], {logInfo.DateTime.TimeOfDay}");
            Console.WriteLine($"\t{string.Join('/', isNestable ? logInfo.ProcessNestFrame ?? [] : [])}, {logInfo.Message}");
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
