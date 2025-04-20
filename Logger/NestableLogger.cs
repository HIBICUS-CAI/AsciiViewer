using System.Diagnostics;

namespace Logger;

/// <summary>
/// ネスト可能なロガー
/// </summary>
///
/// <remarks>
/// ネストは二つの部分で構成されている、モジュールネストと処理ネスト<br/>
/// モジュールネストは、処理を行う実体、またはその実体が属するモジュールを表す<br/>
/// 処理ネストは、今行っている処理の段階を表す
/// </remarks>
///
/// <remarks>
/// ネストを作成するには、<see cref="LogNest{TLoggerInstanceType}"/>を使用する
/// </remarks>
public class NestableLogger
{
    /// <summary>
    /// ネストされた処理単位の情報
    /// </summary>
    [DebuggerDisplay("ProcessNestName = {Name}")]
    internal class ProcessNest
    {
        /// <summary>
        /// ネストされた処理の名前
        /// </summary>
        internal string? Name { get; init; }
    }

    /// <summary>
    /// ネストされたログ情報、ログメッセージ単位で保持される
    /// </summary>
    private class NestLogInfo
    {
        /// <summary>
        /// ログが記録されたカテゴリの名前
        /// </summary>
        public string? Category { get; internal init; }
        /// <summary>
        /// ログが記録された時点の処理ネストの名前の配列、階層降順で格納される
        /// </summary>
        public string[]? ProcessNestFrame { get; internal init; }
        /// <summary>
        /// ログが記録された日時
        /// </summary>
        public DateTime DateTime { get; } = DateTime.Now;
        /// <summary>
        /// ログメッセージ
        /// </summary>
        public string? Message { get; internal init; }
    }

    /// <summary>
    /// ロガーのカテゴリ
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// ネストされた処理名のスタック
    /// </summary>
    internal Stack<ProcessNest> ProcessNests { get; } = new();

    /// <summary>
    /// 記録されたログ情報のキュー
    /// </summary>
    private Queue<NestLogInfo> LogQueue { get; } = new();

    /// <summary>
    /// 指定のネストを使用するかどうか、<see cref="LogNest.Logger"/>を使用している場合はtrueになり、そのオブジェクトのネストをログ記録に使用する<br/>
    /// 一回任意のログを記録した後、falseに戻る<br/>
    /// 処理やモジュールのネスト流れが追いづらくなるため、できるだけ使用しないことを推奨
    /// </summary>
    internal (bool enabled, ProcessNest targetProcessNest) UseSpecificNestObject { get; set; } = (false, null)!;

    /// <summary>
    /// ログを記録する
    /// #TODO 簡易版、とりあえずすぐに出す
    /// </summary>
    /// <param name="message">ログメッセージ</param>
    public void Log(string message)
    {
        var processNests = ProcessNests;
        if (UseSpecificNestObject.enabled)
        {
            processNests = new Stack<ProcessNest>();
            foreach (var existedProcessNest in ProcessNests.Reverse())
            {
                processNests.Push(existedProcessNest);
                if (existedProcessNest == UseSpecificNestObject.targetProcessNest)
                {
                    break;
                }
            }

            // リセット
            UseSpecificNestObject = (false, null)!;
        }

        var nestLogInfo = new NestLogInfo
        {
            Category = Category,
            ProcessNestFrame = processNests.Reverse().Select(process => process.Name).ToArray()!,
            Message = message,
        };

        LogQueue.Enqueue(nestLogInfo);
        // #TODO 簡易版でとりあえずすぐに出す
        var info = LogQueue.Dequeue();
        Debug.WriteLine($"[{info.Category}], {info.DateTime.TimeOfDay}");
        Debug.WriteLine($"\t{string.Join('/', info.ProcessNestFrame!)}, {info.Message}");
    }
}
