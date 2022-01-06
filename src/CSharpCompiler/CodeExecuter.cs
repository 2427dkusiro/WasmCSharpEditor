using System.Reflection;
using System.Runtime.Loader;

namespace CSharpCompiler;

/// <summary>
/// コードを実行する機能を提供します。
/// </summary>
internal class CodeExecuter
{
    /// <summary>
    /// コードを実行します。
    /// </summary>
    /// <param name="compileResult">実行するコンパイル結果。</param>
    /// <param name="stdIn">標準入力。</param>
    /// <param name="stdOut">標準出力。</param>
    /// <param name="stdError">標準エラー出力。</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static RunCodeResult RunCode(byte[] compileResult, TextReader stdIn, TextWriter stdOut, TextWriter stdError)
    {
        var memoryStream = new MemoryStream();
        memoryStream.Write(compileResult);
        memoryStream.Seek(0, SeekOrigin.Begin);
        Assembly asm = AssemblyLoadContext.Default.LoadFromStream(memoryStream) ?? throw new ArgumentException("コンパイル失敗したコンパイル結果を実行することはできません", nameof(compileResult));

        MethodInfo redirectMethod = asm.GetTypes().First(type => type.Name == "__CompilerGenerated").GetMethod("__RedirectStd", BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException("コード注入が正常に行われていません。標準入出力リダイレクトメソッドが検出できません。");

        redirectMethod.Invoke(null, new object[] { stdIn, stdOut, stdError });

        if (CSharpCompiler.TryGetMainMethod(asm, out MethodInfo? info))
        {
            try
            {
                info.Invoke(null, null);
                return new RunCodeResult()
                {
                    IsSuccessed = true,
                    OccurredException = null,
                };
            }
            catch (TargetInvocationException ex)
            {
                return new RunCodeResult()
                {
                    IsSuccessed = false,
                    OccurredException = new BlazorTask.Dispatch.WorkerException(ex.InnerException.Message, ex.InnerException.StackTrace, ex.InnerException.Source, ex.InnerException.GetType().Name),
                };
            }
        }
        else
        {
            throw new ArgumentException("メインメソッドが検出できていないコンパイル結果を実行することはできません", nameof(compileResult));
        }
    }
}
