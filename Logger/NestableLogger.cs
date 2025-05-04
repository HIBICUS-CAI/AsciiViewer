using System.Diagnostics;

namespace Logger;
/// <summary>
/// ネスト可能なロガー
/// </summary>
///
/// <remarks>
/// ロガーは処理ネストを管理している<br/>
/// 処理ネストは、今行っている処理の段階を表す
/// </remarks>
///
/// <remarks>
/// ネストを作成するには、<see cref="LogNest{TLoggerInstanceType}"/>を使用する
/// </remarks>
public class NestableLogger : StandardLogger
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
    /// ロガーの機能マスク、<see cref="NestableLogger"/>的には<see cref="LoggerFeature.Nestable"/>を追加
    /// </summary>
    internal override LoggerFeature LoggerFeature => base.LoggerFeature | LoggerFeature.Nestable;

    /// <summary>
    /// ネストされた処理名のスタック
    /// </summary>
    internal Stack<ProcessNest> ProcessNests { get; } = new();

    /// <summary>
    /// 指定のネストを使用するかどうか、<see cref="LogNest.Logger"/>を使用している場合はtrueになり、そのオブジェクトのネストをログ記録に使用する<br/>
    /// 一回任意のログを記録した後、falseに戻る<br/>
    /// 処理やモジュールのネスト流れが追いづらくなるため、できるだけ使用しないことを推奨
    /// </summary>
    internal (bool enabled, object targetProcessNest) UseSpecificNestObject { get; set; } = (false, null)!;

    /// <summary>
    /// ネストを作成
    /// </summary>
    /// <param name="processNest">処理ネストの名前</param>
    /// <returns>ネストのハンドル</returns>
    internal object PushNest(string? processNest)
    {
        var newProcessNest = new ProcessNest { Name = processNest ?? $"Process_{ProcessNests.Count}" };
        ProcessNests.Push(newProcessNest);
        return (newProcessNest, ProcessNests.Count);
    }

    /// <summary>
    /// 指定したネストのハンドルが最上層かを確認
    /// </summary>
    /// <param name="nestHandle">ネストのハンドル</param>
    /// <returns>最上層かどうか</returns>
    internal bool IsTopNest(object nestHandle)
    {
        return nestHandle is (ProcessNest processNest, int count) && ProcessNests.Peek() == processNest && ProcessNests.Count == count;
    }

    /// <summary>
    /// ログを記録する
    /// </summary>
    /// <param name="message">ログメッセージ</param>
    /// <param name="logElement">ログエレメント、指定した場合は前述の内容を上書きする、基本派生元ロガーに渡して加工させてもらう用</param>
    public override void Log(string message, LogElement? logElement = null)
    {
        var processNests = ProcessNests;
        if (UseSpecificNestObject is { enabled: true, targetProcessNest: (ProcessNest nest, int count) } && ProcessNests.Contains(nest))
        {
            processNests = new Stack<ProcessNest>();
            int index = 0;
            foreach (var existedProcessNest in ProcessNests.Reverse())
            {
                processNests.Push(existedProcessNest);
                if (++index >= count)
                {
                    break;
                }
            }

            // リセット
            UseSpecificNestObject = (false, null)!;
        }

        logElement ??= new LogElement();
        logElement.ProcessNestFrame = processNests.Reverse().Select(process => process.Name).ToArray()!;
        base.Log(message, logElement);
    }
}
