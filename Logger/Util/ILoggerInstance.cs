namespace Logger.Util;
/// <summary>
/// <see cref="NestableLogger"/>取得用の便利インターフェース
/// </summary>
///
/// <remarks>
/// ロガークラスはこのインターフェースを実装することで、リフレクション経由で統一の関数<see cref="AssemblyHelper.FindLoggerFromAssembly"/>ロガーを取得できるようになる<br/>
/// リフレクションを使わずにロガーを取得する場合は、他の方法で<see cref="NestableLogger"/>のインスタンスを直接取得すること<br/>
/// 継承する場合、一つのアセンブリに一つだけ実装できる<br/>
/// なので、利用するのなら、アセンブリの名義で実装することを推奨<br/>
/// </remarks>
///
/// <example>
/// <code>
/// namespace Logger.Example
/// {
///     internal abstract class ExampleLogger : ILoggerInstance
///     {
///         static NestableLogger s_logger = new();
///         static NestableLogger Get() => s_logger;
///     }
///     static class Program
///     {
///         void Main()
///         {
///             // こう見ると、リフレクションで取得するのが面倒くさいだが、<see cref="LogNestObject"/>を生成する際に書くコードが省けるので、少しは楽になるかも
///             Util.AssemblyHelper.FindLoggerFromAssembly(Assembly.GetExecutingAssembly()).Log("hello world");
///         }
///     }
/// }
/// </code>
/// </example>
public interface ILoggerInstance
{
    /// <summary>
    /// <see cref="NestableLogger"/>を取得する
    /// </summary>
    /// <returns><see cref="NestableLogger"/>のインスタンス</returns>
    static abstract NestableLogger Get();
}
