namespace Logger;

/// <summary>
/// ログ出力時に使用するパラメータ
/// </summary>
public class LogPrintArgs
{
    /// <summary>
    /// ログの情報
    /// </summary>
    public required LogElement LogElement { get; init; }
    /// <summary>
    /// ロガーの機能マスク
    /// </summary>
    public LoggerFeature LoggerFeature { get; init; }
}

/// <summary>
/// ログ出力を取りまとめるクラス
/// </summary>
public static class LogPrinter
{
    /// <summary>
    /// 記録されたログ情報のキュー
    /// </summary>
    private static readonly Queue<LogElement> s_logElements = new();

    /// <summary>
    /// ログプリントのイベント、出力の仕方はここに登録すべき
    /// </summary>
    public static event EventHandler<LogPrintArgs>? Print;

    /// <summary>
    /// ログを記録して、出力を待つ
    /// </summary>
    /// <param name="logElement">ログの情報</param>
    /// <param name="loggerFeature">ロガーの機能マスク</param>
    internal static void RecordLog(LogElement logElement, LoggerFeature loggerFeature)
    {
        s_logElements.Enqueue(logElement);
        // #TODO 簡易版でとりあえずすぐに出す
        Print?.Invoke(typeof(LogPrinter), new LogPrintArgs
        {
            LogElement = s_logElements.Dequeue(),
            LoggerFeature = loggerFeature
        });
    }
}
