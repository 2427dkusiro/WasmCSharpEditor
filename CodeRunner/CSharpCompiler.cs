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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeRunner.CodeInjection;

namespace CodeRunner
{
    public class CSharpCompiler
    {
        private readonly NetworkAssemblyLoader networkAssemblyLoader;
        private IEnumerable<MetadataReference> metadataReferences;
        private SyntaxTree injectCode;

        public CSharpCompiler(HttpClient httpClient)
        {
            networkAssemblyLoader = new NetworkAssemblyLoader(httpClient);
        }

        public async Task<CompileResult> Compile(string code)
        {
            var parseOption = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp9);
            var syntaxTree = CSharpSyntaxTree.ParseText(code, parseOption);

            if (injectCode is null)
            {
                injectCode = CSharpSyntaxTree.ParseText(InjectCode.Code);
            }

            if (metadataReferences is null)
            {
                metadataReferences = await networkAssemblyLoader.LoadAsync();
            }

            var compileOption = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compile = CSharpCompilation.Create("Hoge", new[] { syntaxTree, injectCode }, metadataReferences, compileOption);

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
                        if (i == 0)
                        {
                            Debug.WriteLine($"[Info]既知の問題「一回目のコンパイルが失敗する」が発生しました。再試行します...");
                        }
                        Debug.WriteLine($"コンパイラに内部的な問題が生じました、再試行します...{i.ToString()}回目");
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
                    foreach(var d in emitResult.Diagnostics)
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