namespace Logger;
/// <summary>
/// <see cref="StandardLogger"/>取得用のインターフェース
/// </summary>
///
/// <remarks>
/// ロガークラスはこのインターフェースを実装することで、実際に使用するロガーを選別すること、そして<see cref="LogNest{TLoggerInstanceType}"/>でそれを取得できる<br/>
/// ログを出したいモジュールに対して、基本こちらを継承する必要がある
/// </remarks>
///
/// <remarks>
/// staticクラスに非対応、使いたい場合はクラスに対してのstaticを外して継承する
/// </remarks>
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
    /// <see cref="StandardLogger"/>を取得する
    /// </summary>
    /// <returns><see cref="StandardLogger"/>のインスタンス</returns>
    static abstract StandardLogger Get();
}
