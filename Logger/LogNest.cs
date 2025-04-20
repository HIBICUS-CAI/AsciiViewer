namespace Logger;

/// <summary>
/// 明示的に<see cref="NestableLogger"/>ロガーを指定してネストを作成するクラス、Disposeでネストを解放する<br/>
/// using文でネストを作成することを推奨
/// </summary>
///
/// <remarks>
/// <see cref="NestableLogger"/>は基本モジュールごとに静的変数として宣言すべきなので、こちらを使用することは少ないはず<br/>
/// ただデバッグ用など、一時的に特殊のロガーを使用したい場合、こちらを使用することで冗長な変数・クラス定義が省ける
/// </remarks>
///
/// <example>
/// <code>
/// NestableLogger nestableLogger = new NestableLogger();
/// using (var logNest1 = new LogNest(nestableLogger, "親処理"))
/// {
///     nestableLogger.Log("hello world");  // 親処理のログとして記録
///     using (new LogNest(nestableLogger, "子処理"))
///     {
///         nestableLogger.Log("hello world again");    // 子処理のログとして記録
///         // 明示的にどのネストを使用するかのも指定できるが、処理の流れを追うのが難しくなるので、あまり推奨しない
///         logNest1.Logger.Log("hello world again and again");
///     }
/// }
/// </code>
/// </example>
public class LogNest : IDisposable
{
    /// <summary>
    /// ネストされた処理のハンドル
    /// </summary>
    private readonly object m_processNestHandle;

    /// <summary>
    /// ロガーのインスタンス
    /// </summary>
    private readonly NestableLogger m_logger;

    /// <summary>
    /// 指定したネストを使用するようにロガーを取得する<br/>
    /// 処理の流れが追いづらくなるので、基本こちらを使用せず、ロガー自体を使用すべき
    /// </summary>
    public NestableLogger Logger
    {
        get
        {
            if (!m_logger.IsTopNest(m_processNestHandle))
            {
                m_logger.UseSpecificNestObject = (true, m_processNestHandle);
            }

            return m_logger;
        }
    }

    /// <summary>
    /// <see cref="Logger.ILoggerInstance"/>を実装しているロガーを使用するコンストラクタ
    /// </summary>
    /// <param name="logger">ロガーのインスタンス</param>
    /// <param name="processNest">処理のネスト情報、指定しない場合は呼び出し元の関数名になる</param>
    public LogNest(NestableLogger logger, string? processNest = null)
    {
        m_logger = logger;
        m_processNestHandle = m_logger.PushNest(processNest);
    }

    /// <summary>
    /// ネストを解放する
    /// </summary>
    public void Dispose()
    {
        _ = m_logger.ProcessNests.Pop();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// ログのネストを作成用のクラス、Disposeでネストを解放する<br/>
/// using文でネストを作成することを推奨
/// </summary>
/// <typeparam name="TLoggerInstanceType"><see cref="ILoggerInstance"/>ロガーインスタンスを実装している型</typeparam>
///
/// <example>
/// <code>
/// abstract class Viewer : ILoggerInstance
/// {
///     public static NestableLogger Logger { get; } = new() { Category = nameof(Viewer) };
///     public static NestableLogger Get() => Logger;
/// }
/// using (new LogNest&lt;Logger&gt;("親処理"))
/// {
///     Viewer.Logger.Log("hello world");   // 親処理のログとして記録
///     using (new LogNest&lt;Logger&gt;("子処理"))
///     {
///         Viewer.Logger.Log("hello world again"); // 子処理のログとして記録
///     }
/// }
/// </code>
/// </example>
///
/// <param name="processNest">処理のネスト情報、指定しない場合は呼び出し元の関数名になる</param>
public class LogNest<TLoggerInstanceType>(string? processNest = null)
    : LogNest(TLoggerInstanceType.Get(), processNest)
    where TLoggerInstanceType : ILoggerInstance;
