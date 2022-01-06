namespace CSharpCompiler;

/// <summary>
/// C#コードのテンプレートを提供します。
/// </summary>
public static class CodeTempletes
{
    /// <summary>
    /// 現在時刻を表示するサンプルコードを取得します。
    /// </summary>
    public static string GetNowTimeSampleCode =>
@"using System;
class Program
{
    static void Main()
    {
        Console.WriteLine(DateTime.Now);
    }
}";

    /// <summary>
    /// 空のMain関数を含むサンプルコードを取得します。
    /// </summary>
    public static string TempleteCode =>
@"using System;
class Program
{
    static void Main()
    {
        // Write your code here
    }
}";

    /// <summary>
    /// コンパイラとC#言語のバージョンを取得するコードを取得します。
    /// </summary>
    public static string GetVersionCode =>
@"using System;
class Program
{
    static void Main()
    {
        #error version
    }
}";
}