using System.Reflection;

namespace Logger.Util;
/// <summary>
/// ロガーをリフレクションで取得するためのヘルパークラス
/// </summary>
public static class AssemblyHelper
{
    /// <summary>
    /// 指定のアセンブリから<see cref="ILoggerInstance"/>を実装しているロガーを取得する
    /// </summary>
    /// <param name="assembly">ロガーを取得するアセンブリ</param>
    /// <returns>ロガーのインスタンス</returns>
    /// <exception cref="ApplicationException">呼び出し元のアセンブリ内に<see cref="ILoggerInstance"/>のロガーが定義していない、または複数定義されている</exception>
    public static NestableLogger FindLoggerFromAssembly(Assembly assembly)
    {
        var types = assembly
            .GetTypes()
            .Where(type => typeof(ILoggerInstance).IsAssignableFrom(type) && !type.IsInterface)
            .ToList();

        return types.Count switch
        {
            // #TODO ちゃんと例外を定義する
            0 => throw new ApplicationException(
                $"cannot find any ILoggerInstance instance declared in assembly {assembly.FullName}"),

            // #TODO ちゃんと例外を定義する
            > 1 => throw new ApplicationException(
                $"find multiply ILoggerInstance instance declared in assembly {assembly.FullName}"),

            // ILoggerInstanceを継承している時点でstaticなGetメソッドがあるはずなので、NULLチェック一切しない
            _ => (types[0].GetMethod(nameof(ILoggerInstance.Get))!.Invoke(null, null) as NestableLogger)!
        };
    }
}
