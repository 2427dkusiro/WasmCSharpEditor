using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Loader;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Net.Http;
using System.Diagnostics;

using CodeRunner.DllLoader;
using CodeRunner.CodeInjection;

namespace CodeRunner
{
    public class CSharpCompiler
    {
        private readonly NetworkAssemblyLoader networkAssemblyLoader;
        private static IEnumerable<MetadataReference> metadataReferences;
        private static SyntaxTree injectCode;

        public CSharpCompiler(HttpClient httpClient, string indexPath)
        {
            networkAssemblyLoader = new NetworkAssemblyLoader(httpClient, indexPath);
        }

        public async Task<CompileResult> Compile(string code)
        {
            var parseOption = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp9);
            var syntaxTree = await Task.Run(() => CSharpSyntaxTree.ParseText(code, parseOption));

            if (injectCode is null)
            {
                injectCode = await Task.Run(() => CSharpSyntaxTree.ParseText(InjectCode.Code));
            }

            if (metadataReferences is null)
            {
                metadataReferences = await networkAssemblyLoader.LoadAsync();
            }

            var compileOption = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compile = await Task.Run(() => CSharpCompilation.Create("__HogeAssembly", new[] { syntaxTree, injectCode }, metadataReferences, compileOption));

            Microsoft.CodeAnalysis.Emit.EmitResult emitResult = default;
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        emitResult = compile.Emit(memoryStream);
                        break;
                    }
                    catch (Exception)
                    {
                        memoryStream.Dispose();
                        memoryStream = new MemoryStream();
                        await Task.Delay(250);
                        Debug.WriteLine($"コンパイラに内部的な問題が生じました、再試行します...{(i + 1).ToString()}回目");
                        continue;
                    }
                }

                if (emitResult.Success)
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var asm = AssemblyLoadContext.Default.LoadFromStream(memoryStream);

                    return new CompileResult(true, emitResult.Diagnostics, asm);
                }
                else
                {
                    foreach (var d in emitResult.Diagnostics)
                    {
                        Console.WriteLine(d.ToString());
                    }
                    return new CompileResult(false, emitResult.Diagnostics, null);
                }
            }
            finally
            {
                memoryStream.Dispose();
            }
        }
    }

    public class CompileResult
    {
        public CompileResult(bool isSuccessed, IEnumerable<Diagnostic> diagnostics, Assembly assembly)
        {
            IsSuccessed = isSuccessed;
            Diagnostics = diagnostics;
            Assembly = assembly;
        }

        public bool IsSuccessed { get; }

        public IEnumerable<Diagnostic> Diagnostics { get; }

        public Assembly Assembly { get; }
    }
}