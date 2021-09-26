using CodeRunner;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using System.Reflection;
using System.Diagnostics;

namespace WasmCsTest.Codes
{
    public class CodeEditorContext
    {
        private readonly CSharpCompiler cSharpCompiler;
        private readonly string indexPath = "res/DLLIndex.json";

        public CodeEditorContext(HttpClient httpClient)
        {
            cSharpCompiler = new CSharpCompiler(httpClient, indexPath);
        }

        private readonly object isCodeRunningSync = new object();
        public bool IsCodeCompiling { get; private set; }

        public CompileResult CompileResult { get; private set; }
        public string CompileTime { get; private set; }

        private Stopwatch stopwatch = new Stopwatch();
        public async Task<CompileResult> Compile(string code)
        {
            lock (isCodeRunningSync)
            {
                if (IsCodeCompiling)
                {
                    return null;
                }
                else
                {
                    IsCodeCompiling = true;
                }
            }
            stopwatch.Restart();
            var result = await Task.Run(async () => await cSharpCompiler.Compile(code));
            CompileResult = result;
            stopwatch.Stop();
            CompileTime = $"{stopwatch.ElapsedMilliseconds}ms";

            lock (isCodeRunningSync)
            {
                IsCodeCompiling = false;
            }
            return CompileResult;
        }

        public async Task RunCode(CompileResult compileResult)
        {
            if (compileResult is null)
            {
                throw new ArgumentNullException();
            }

            var asm = compileResult.Assembly;
            if (asm is null)
            {
                throw new ArgumentException();
            }

            var redirectMethod = asm.GetTypes().First(type => type.Name == "__CompilerGenerated").GetMethod("__RedirectStd", BindingFlags.Public | BindingFlags.Static);
            redirectMethod.Invoke(null, new object[] { StdIn, StdOut, StdError });

            var mainMethod = asm.GetTypes().Select(x => new[] { x.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static), x.GetMethod("Main", BindingFlags.Public | BindingFlags.Static) }).SelectMany(x => x).FirstOrDefault(x => x is not null);
            if (mainMethod is null)
            {
                // Mainメソッドが定義されていない！
            }
            else
            {
                await Task.Run(() => mainMethod.Invoke(null, null));
            }
        }

        public TextWriter StdOut { get; set; }
        public TextWriter StdError { get; set; }
        public TextReader StdIn { get; set; }
    }
}