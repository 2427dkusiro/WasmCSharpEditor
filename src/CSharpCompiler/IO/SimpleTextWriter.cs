using System.Text;

namespace CSharpCompiler.IO;

/// <summary>
/// <see cref="TextWriter"/> の実装を簡略化します。<see cref="WriteString(string)"/> を実装するだけで利用できます。
/// </summary>
public abstract class SimpleTextWriter : TextWriter
{
    /// <inheritdoc />
    public override Encoding Encoding => Encoding.GetEncoding("UTF-16");

    /// <inheritdoc />
    public override string NewLine
    {
        get => Environment.NewLine;
#pragma warning disable CS8765 // パラメーターの型の NULL 値の許容が、オーバーライドされたメンバーと一致しません。おそらく、NULL 値の許容の属性が原因です。
        set => _ = value;
#pragma warning restore CS8765 // パラメーターの型の NULL 値の許容が、オーバーライドされたメンバーと一致しません。おそらく、NULL 値の許容の属性が原因です。
    }

    /// <summary>
    /// 派生クラスでオーバーライドされた場合は、文字列を書き込みます。
    /// </summary>
    /// <param name="value"></param>
    public abstract void WriteString(string? value);

    /// <summary>
    /// 値を書き込みます。
    /// </summary>
    /// <typeparam name="T">書き込む値の型。</typeparam>
    /// <param name="value">書き込む値。</param>
    protected void WriteGeneric<T>(T value)
    {
        var str = value?.ToString();
        WriteString(str);
    }

    /// <summary>
    /// 値を書き込み、続いて改行文字を書き込みます。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    protected void WriteLineGeneric<T>(T value)
    {
        var str = value?.ToString();
        WriteString(str + NewLine);
    }

    #region Writes
    /// <inheritdoc/>
    public override void Write(bool value)
    {
        WriteGeneric(value);
    }

    /// <inheritdoc/>
    public override void Write(char value)
    {
        WriteGeneric(value);
    }

    /// <inheritdoc/>
    public override void Write(char[] buffer, int index, int count)
    {
        WriteGeneric(new string(buffer, index, count));
    }

    /// <inheritdoc/>
    public override void Write(char[]? buffer)
    {
        WriteGeneric(new string(buffer));
    }

    /// <inheritdoc/>
    public override void Write(decimal value)
    {
        WriteGeneric(value);
    }

    /// <inheritdoc/>
    public override void Write(double value)
    {
        WriteGeneric(value);
    }

    /// <inheritdoc/>
    public override void Write(float value)
    {
        WriteGeneric(value);
    }

    /// <inheritdoc/>
    public override void Write(int value)
    {
        WriteGeneric(value);
    }

    /// <inheritdoc/>
    public override void Write(long value)
    {
        WriteGeneric(value);
    }

    /// <inheritdoc/>
    public override void Write(object? value)
    {
        WriteGeneric(value);
    }

    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<char> buffer)
    {
        WriteGeneric(new string(buffer));
    }

    /// <inheritdoc/>
    public override void Write(string format, object? arg0)
    {
        WriteGeneric(string.Format(format, arg0));
    }

    /// <inheritdoc/>
    public override void Write(string format, object? arg0, object? arg1)
    {
        WriteGeneric(string.Format(format, arg0, arg1));
    }

    /// <inheritdoc/>
    public override void Write(string format, object? arg0, object? arg1, object? arg2)
    {
        WriteGeneric(string.Format(format, arg0, arg1, arg2));
    }

    /// <inheritdoc/>
    public override void Write(string format, params object?[] arg)
    {
        WriteGeneric(string.Format(format, arg));
    }

    /// <inheritdoc/>
    public override void Write(string? value)
    {
        WriteGeneric(value);
    }

    /// <inheritdoc/>
    public override void Write(StringBuilder? value)
    {
        WriteGeneric(value);
    }

    /// <inheritdoc/>
    public override void Write(uint value)
    {
        WriteGeneric(value);
    }

    /// <inheritdoc/>
    public override void Write(ulong value)
    {
        WriteGeneric(value);
    }
    #endregion

    #region WriteLines

    /// <inheritdoc/>
    public override void WriteLine()
    {
        WriteString(NewLine);
    }

    /// <inheritdoc/>
    public override void WriteLine(bool value)
    {
        WriteLineGeneric(value);
    }

    /// <inheritdoc/>
    public override void WriteLine(char value)
    {
        WriteLineGeneric(value);
    }

    /// <inheritdoc/>
    public override void WriteLine(char[] buffer, int index, int count)
    {
        WriteLineGeneric(new string(buffer, index, count));
    }

    /// <inheritdoc/>
    public override void WriteLine(char[]? buffer)
    {
        WriteLineGeneric(new string(buffer));
    }

    /// <inheritdoc/>
    public override void WriteLine(decimal value)
    {
        WriteLineGeneric(value);
    }

    /// <inheritdoc/>
    public override void WriteLine(double value)
    {
        WriteLineGeneric(value);
    }

    /// <inheritdoc/>
    public override void WriteLine(float value)
    {
        WriteLineGeneric(value);
    }

    /// <inheritdoc/>
    public override void WriteLine(int value)
    {
        WriteLineGeneric(value);
    }

    /// <inheritdoc/>
    public override void WriteLine(long value)
    {
        WriteLineGeneric(value);
    }

    /// <inheritdoc/>
    public override void WriteLine(object? value)
    {
        WriteLineGeneric(value);
    }

    /// <inheritdoc/>
    public override void WriteLine(ReadOnlySpan<char> buffer)
    {
        WriteLineGeneric(new string(buffer));
    }

    /// <inheritdoc/>
    public override void WriteLine(string format, object? arg0)
    {
        WriteLineGeneric(string.Format(format, arg0));
    }

    /// <inheritdoc/>
    public override void WriteLine(string format, object? arg0, object? arg1)
    {
        WriteLineGeneric(string.Format(format, arg0, arg1));
    }

    /// <inheritdoc/>
    public override void WriteLine(string format, object? arg0, object? arg1, object? arg2)
    {
        WriteLineGeneric(string.Format(format, arg0, arg1, arg2));
    }

    /// <inheritdoc/>
    public override void WriteLine(string format, params object?[] arg)
    {
        WriteLineGeneric(string.Format(format, arg));
    }

    /// <inheritdoc/>
    public override void WriteLine(string? value)
    {
        WriteLineGeneric(value);
    }

    /// <inheritdoc/>
    public override void WriteLine(StringBuilder? value)
    {
        WriteLineGeneric(value);
    }

    /// <inheritdoc/>
    public override void WriteLine(uint value)
    {
        WriteLineGeneric(value);
    }

    /// <inheritdoc/>
    public override void WriteLine(ulong value)
    {
        WriteLineGeneric(value);
    }

    #endregion
}