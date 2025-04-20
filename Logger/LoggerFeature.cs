namespace Logger;
/// <summary>
/// ロガーの機能フラグ
/// </summary>
[Flags]
public enum LoggerFeature
{
    /// <summary>
    /// 何もない、標準的なロガー
    /// </summary>
    Standard = 0,
    /// <summary>
    /// 処理をネストできるロガー
    /// </summary>
    Nestable = 1 << 0,
}
