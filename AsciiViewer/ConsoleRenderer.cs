namespace AsciiViewer;

/// <summary>
/// コンソールの描画処理を提供するモジュール
/// </summary>
public class ConsoleRenderer
{
    private readonly TextWriter _output;

    public ConsoleRenderer() : this(Console.Out)
    {
    }

    public ConsoleRenderer(TextWriter output)
    {
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    /// <summary>
    /// AsciiSurfaceの内容をコンソールに描画する
    /// </summary>
    public void Render(AsciiSurface surface)
    {
        if (surface == null)
        {
            throw new ArgumentNullException(nameof(surface));
        }

        // バッファの内容を取得して出力
        string content = surface.GetBufferAsString();
        _output.Write(content);
    }

    /// <summary>
    /// AsciiSurfaceの内容を行ごとに描画する
    /// </summary>
    public void RenderLine(AsciiSurface surface)
    {
        if (surface == null)
        {
            throw new ArgumentNullException(nameof(surface));
        }

        string content = surface.GetBufferAsString();
        _output.WriteLine(content);
    }

    /// <summary>
    /// カスタム描画処理
    /// </summary>
    public void RenderCustom(Action<TextWriter> customDrawAction)
    {
        if (customDrawAction == null)
        {
            throw new ArgumentNullException(nameof(customDrawAction));
        }

        customDrawAction(_output);
    }

    /// <summary>
    /// コンソールをクリアする
    /// </summary>
    public void Clear()
    {
        Console.Clear();
    }
}
