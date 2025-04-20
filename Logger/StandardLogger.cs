namespace Logger;
/// <summary>
/// 標準のロガー
/// </summary>
///
/// <remarks>
/// カテゴリ名を指定できる<br/>
/// カテゴリ名は、処理を行う実体、またはその実体が属するモジュールを表す
/// </remarks>
public class StandardLogger
{
    /// <summary>
    /// ロガーのカテゴリ
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// 記録されたログ情報のキュー
    /// </summary>
    private Queue<(LogElement logElement, LoggerFeature loggerFeature)> LogQueue { get; } = new();

    /// <summary>
    /// ロガーの機能マスク、<see cref="StandardLogger"/>的には<see cref="LoggerFeature.Standard"/>
    /// </summary>
    internal virtual LoggerFeature LoggerFeature => LoggerFeature.Standard;

    /// <summary>
    /// ログを記録する
    /// </summary>
    /// <param name="message">ログメッセージ</param>
    /// <param name="logElement">ログエレメント、指定した場合は前述の内容を上書きする、基本派生元ロガーに渡して加工させてもらう用</param>
    public virtual void Log(string message, LogElement? logElement = null)
    {
        logElement ??= new LogElement();
        logElement.Category = Category;
        logElement.Message = message;
        _UploadLog(logElement);
    }

    /// <summary>
    /// 記録したログをアップロード
    /// </summary>
    /// <param name="logElement"></param>
    private void _UploadLog(LogElement logElement)
    {
        LogQueue.Enqueue((logElement, LoggerFeature));
        // #TODO 簡易版でとりあえずすぐに出す
        var (info, _) = LogQueue.Dequeue();
        System.Diagnostics.Debug.WriteLine($"[{info.Category}], {info.DateTime.TimeOfDay}");
        System.Diagnostics.Debug.WriteLine($"\t{string.Join('/', info.ProcessNestFrame ?? [])}, {info.Message}");
    }
}
