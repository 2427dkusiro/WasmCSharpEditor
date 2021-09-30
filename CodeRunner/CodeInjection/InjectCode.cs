namespace CodeRunner.CodeInjection
{
    public static class InjectCode
    {
        public static string Code { get; } = @"namespace __CompilerGenerated
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