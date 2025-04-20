using System.Diagnostics;
using System.Reflection;

namespace Logger;
/// <summary>
/// ログのネストを作成用のクラス、Disposeでネストを解放する<br/>
/// using文でネストを作成することを推奨
/// </summary>
///
/// <example>
/// <code>
/// NestableLogger nestableLogger = new NestableLogger();
/// using (var logNest1 = new LogNestObject("親処理"))
/// {
///     nestableLogger.Log("hello world");  // 親処理のログとして記録
///     using (var logNest2 = new LogNestObject("子処理"))
///     {
///         nestableLogger.Log("hello world again");    // 子処理のログとして記録
///         // 明示的にどのネストを使用するかのも指定できるが、処理の流れを追うのが難しくなるので、あまり推奨しない
///         logNest1.Logger.Log("hello world again and again");
///     }
/// }
/// </code>
/// </example>
public class LogNestObject : IDisposable
{
    /// <summary>
    /// ネストされた処理単位の情報
    /// </summary>
    internal NestableLogger.ProcessNest ProcessNest { get; }
    /// <summary>
    /// ネストされたモジュール単位の情報
    /// </summary>
    internal NestableLogger.ModuleNest ModuleNest { get; }

    /// <summary>
    /// ロガーのインスタンス
    /// </summary>
    private readonly NestableLogger m_logger;

    /// <summary>
    /// 指定したネストを使用するようにロガーを取得する、それ以外の場合は基本こちらを使用せず、ロガー自体を使用すべき
    /// #NOTE 処理やモジュールのネスト流れが追いづらくなるため、できるだけ使用しないことを推奨
    /// </summary>
    public NestableLogger Logger
    {
        get
        {
            m_logger.UseSpecificNestObject = (true, this);
            return m_logger;
        }
    }

    /// <summary>
    /// ロガー指定付きコンストラクタ
    /// </summary>
    /// <param name="logger">ロガーのインスタンス</param>
    /// <param name="processNest">処理のネスト情報、指定しない場合は呼び出し元の関数名になる</param>
    /// <param name="moduleNest">モジュールのネスト情報、前回のモジュールネストと同じであればそちらにマージする、指定しない場合は呼び出し元の型名になる</param>
    /// <param name="stackFrameIndex">呼び出し元に追跡するためのスタック遡る回数、デフォルトは一層上</param>
    public LogNestObject(NestableLogger logger, string? processNest = null, string? moduleNest = null, int stackFrameIndex = 1)
    {
        m_logger = logger;
        var stackFrame = new StackTrace().GetFrame(stackFrameIndex)!;
        var callingMethod = stackFrame.GetMethod()!;
        var callingType = callingMethod.DeclaringType!;
        var processNestObject = new NestableLogger.ProcessNest { Name = processNest ?? callingMethod.Name };
        moduleNest ??= callingType.FullName ?? "GenericType";
        if (!m_logger.ModuleNests.TryPeek(out var existModuleNest) || existModuleNest.Name != moduleNest)
        {
            existModuleNest = new NestableLogger.ModuleNest { Name = moduleNest };
            m_logger.ModuleNests.Push(existModuleNest);
        }
        existModuleNest.ProcessNests.Push(processNestObject);

        ModuleNest = existModuleNest;
        ProcessNest = processNestObject;
    }

    /// <summary>
    /// <see cref="Util.ILoggerInstance"/>を実装しているロガーを使用するコンストラクタ
    /// </summary>
    /// <param name="processNest">処理のネスト情報、指定しない場合は呼び出し元の関数名になる</param>
    /// <param name="moduleNest">モジュールのネスト情報、前回のモジュールネストと同じであればそちらにマージする、指定しない場合は呼び出し元の型名になる</param>
    /// <param name="stackFrameIndex">呼び出し元に追跡するためのスタック遡る回数、デフォルトは一層上</param>
    public LogNestObject(string? processNest = null, string? moduleNest = null, int stackFrameIndex = 1)
        : this(Util.AssemblyHelper.FindLoggerFromAssembly(Assembly.GetCallingAssembly()), processNest, moduleNest, stackFrameIndex + 1)
    {
    }

    /// <summary>
    /// ネストを解放する
    /// </summary>
    public void Dispose()
    {
        _ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// ネストを解放するための内部処理
    /// </summary>
    private void _ReleaseUnmanagedResources()
    {
        var topModuleNest = m_logger.ModuleNests.Peek();
        _ = topModuleNest.ProcessNests.Pop();
        if (topModuleNest.ProcessNests.Count == 0)
        {
            _ = m_logger.ModuleNests.Pop();
        }
    }
}
