namespace Logger;
/// <summary>
/// ログの情報を格納するクラス
/// </summary>
public class LogElement
{
    /// <summary>
    /// ログが記録されたカテゴリの名前
    /// </summary>
    public string? Category { get; internal set; }
    /// <summary>
    /// ログが記録された日時
    /// </summary>
    public DateTime DateTime { get; } = DateTime.Now;
    /// <summary>
    /// ログメッセージ
    /// </summary>
    public string? Message { get; internal set; }

    /// <summary>
    /// ログが記録された時点の処理ネストの名前の配列、階層降順で格納される
    /// </summary>
    /// <remarks><see cref="LoggerFeature.Nestable"/>が設定された時のみ参照される</remarks>
    public string[]? ProcessNestFrame { get; internal set; }
}
