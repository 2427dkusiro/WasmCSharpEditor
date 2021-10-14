namespace CodeRunner.CodeInjection
{
    /// <summary>
    /// ユーザーコードへ注入するコードを管理します。
    /// </summary>
    public static class InjectCode
    {
        /// <summary>
        /// 標準入出力をリダイレクトする注入コードを取得します。
        /// </summary>
        public static string RedirectCode { get; } = @"namespace __CompilerGenerated
{
    public static class __CompilerGenerated
    {
        public static void __RedirectStd(
            global::System.IO.TextReader stdInput,
            global::System.IO.TextWriter stdOut,
            global::System.IO.TextWriter stdError
            )
        {
            global::System.Console.SetIn(stdInput);
            global::System.Console.SetOut(stdOut);
            global::System.Console.SetError(stdError);
        }
    }
}";
    }
}

#pragma warning disable CS1591 // 公開されている型またはメンバーの XML コメントがありません
#pragma warning disable IDE1006 // 命名スタイル、ユーザーコードから参照されることを想定していないことを表現。
#pragma warning disable IDE0001 // 命名簡略化、ユーザーコードでどのようなusingをしても動作するようにする
#if DEBUG
namespace __CompilerGenerated
{
    public static class __CompilerGenerated
    {
        public static void __RedirectStd(
            global::System.IO.TextReader stdInput,
            global::System.IO.TextWriter stdOut,
            global::System.IO.TextWriter stdError
            )
        {
            global::System.Console.SetIn(stdInput);
            global::System.Console.SetOut(stdOut);
            global::System.Console.SetError(stdError);
        }
    }
}
#endif
#pragma warning restore CS1591 // 公開されている型またはメンバーの XML コメントがありません
#pragma warning restore IDE1006 // 命名スタイル
#pragma warning restore IDE0001 // 命名簡略化