namespace Logger;
/// <summary>
/// <see cref="NestableLogger"/>取得用のインターフェース
/// </summary>
///
/// <remarks>
/// ロガークラスはこのインターフェースを実装することで、実際に使用するロガーを選別すること、そして<see cref="LogNest{TLoggerInstanceType}"/>でそれを取得できる<br/>
/// ログを出したいモジュールに対して、基本こちらを継承する必要がある
/// </remarks>
///
/// <remarks>staticクラスに非対応</remarks>
///
/// <example>
/// <code>
/// class Hoge : ILoggerInstance
/// {
///     static NestableLogger Logger { get; } = new() { Category = nameof(Hoge) };
///     void Fuga()
///     {
///         Logger.Log("hello");
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
