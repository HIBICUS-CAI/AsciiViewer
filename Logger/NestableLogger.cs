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
/// ネストを作成するには、<see cref="LogNestObject"/>を使用する
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
    /// ネストされたモジュール単位の情報
    /// </summary>
    [DebuggerDisplay("ModuleNestName = {Name}, ProcessNestsSize = {ProcessNests.Count}")]
    internal class ModuleNest
    {
        /// <summary>
        /// ネストされたモジュールの名前
        /// </summary>
        internal string? Name { get; init; }
        /// <summary>
        /// ネストされた処理のスタック
        /// </summary>
        internal Stack<ProcessNest> ProcessNests { get; } = new();
    }

    /// <summary>
    /// ネストされたログ情報、ログメッセージ単位で保持される
    /// </summary>
    private class NestLogInfo
    {
        /// <summary>
        /// ログが記録された時点のモジュールネストの名前の配列、階層降順で格納される
        /// </summary>
        public string[]? ModuleNestFrame { get; internal init; }
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
    /// ネストされたモジュールのスタック
    /// </summary>
    internal Stack<ModuleNest> ModuleNests { get; } = new();

    /// <summary>
    /// 記録されたログ情報のキュー
    /// </summary>
    private Queue<NestLogInfo> LogQueue { get; } = new();

    /// <summary>
    /// 指定のネストを使用するかどうか、<see cref="LogNestObject.Logger"/>を使用している場合はtrueになり、そのオブジェクトのネストをログ記録に使用する<br/>
    /// 一回任意のログを記録した後、falseに戻る<br/>
    /// #NOTE 処理やモジュールのネスト流れが追いづらくなるため、できるだけ使用しないことを推奨
    /// </summary>
    internal (bool enabled, LogNestObject nestObject) UseSpecificNestObject { private get; set; } = (false, null)!;

    /// <summary>
    /// ログを記録する
    /// #TODO 簡易版、とりあえずすぐに出す
    /// </summary>
    /// <param name="message">ログメッセージ</param>
    public void Log(string message)
    {
        NestLogInfo nestLogInfo;
        if (UseSpecificNestObject.enabled)
        {
            var tempModuleNests = new Stack<ModuleNest>();
            foreach (var existedModuleNest in ModuleNests.Reverse())
            {
                tempModuleNests.Push(existedModuleNest);
                if (existedModuleNest == UseSpecificNestObject.nestObject.ModuleNest)
                {
                    break;
                }
            }
            var tempProcessNests = new Stack<ProcessNest>();
            foreach (var existedProcessNest in tempModuleNests.Peek().ProcessNests.Reverse())
            {
                tempProcessNests.Push(existedProcessNest);
                if (existedProcessNest == UseSpecificNestObject.nestObject.ProcessNest)
                {
                    break;
                }
            }
            nestLogInfo = new NestLogInfo
            {
                ModuleNestFrame = tempModuleNests.Reverse().Select(module => module.Name).ToArray()!,
                ProcessNestFrame = tempProcessNests.Reverse().Select(process => process.Name).ToArray()!,
                Message = message,
            };

            // リセット
            UseSpecificNestObject = (false, null)!;
        }
        else
        {
            nestLogInfo = new NestLogInfo
            {
                ModuleNestFrame = ModuleNests.Reverse().Select(module => module.Name).ToArray()!,
                ProcessNestFrame = ModuleNests.Peek().ProcessNests.Reverse().Select(process => process.Name).ToArray()!,
                Message = message,
            };
        }

        LogQueue.Enqueue(nestLogInfo);
        // #TODO 簡易版でとりあえずすぐに出す
        var info = LogQueue.Dequeue();
        Debug.WriteLine($"[{info.ModuleNestFrame![^1]}], {info.DateTime.TimeOfDay}");
        Debug.WriteLine($"\t{string.Join('/', info.ProcessNestFrame!)}, {info.Message}");
    }
}
