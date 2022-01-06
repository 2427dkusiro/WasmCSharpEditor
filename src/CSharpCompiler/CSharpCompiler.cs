using CSharpCompiler.DLLLoader;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
namespace CSharpCompiler;

/// <summary>
/// C#コードのコンパイルを提供します。
/// </summary>
internal class CSharpCompiler
{
    private readonly NetworkAssemblyLoader networkAssemblyLoader;
    private static MetadataReference[]? metadataReferences;
    private static SyntaxTree? injectCode;

    private readonly CSharpParseOptions csharpLangVersion = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);

    /// <summary>
    /// <see cref="CSharpCompiler"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="httpClient">有効な <see cref="HttpClient"/>。コンパイラがDLLを読み込むのに使用します。</param>
    public CSharpCompiler(HttpClient httpClient)
    {
        networkAssemblyLoader = new NetworkAssemblyLoader(httpClient);
    }

    /// <summary>
    /// このコンパイラが初期化されているかどうかを表す値を取得します。
    /// </summary>
    internal bool IsInitialized { get; private set; }

    /// <summary>
    /// コンパイラとC#言語のバージョンを説明する文字列を取得します。コンパイラが初期化されていない場合、<c>null</c> が返されます。
    /// </summary>
    public string? VersionString { get; private set; }

    private readonly object initializeSyncObject = new();
    private bool isInitializing;

    /// <summary>
    /// C#コンパイラを初期化します。
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// 事前にこのメソッドを呼び出すことは、初回コンパイルのオーバーヘッドを隠蔽するのに役立ちます。
    /// </remarks>
    public async Task InitializeAsync()
    {
        if (IsInitialized || isInitializing)
        {
            return;
        }

        isInitializing = true;

        if (injectCode is null)
        {
            injectCode = CSharpSyntaxTree.ParseText(InjectCode.RedirectCode, csharpLangVersion);
        }
        if (metadataReferences is null)
        {
            metadataReferences = (await networkAssemblyLoader.LoadAsync()).ToArray();
        }
        CompileResult result = await CompileAsync(CodeTempletes.GetVersionCode, true);

        IsInitialized = true;

        Diagnostic? versionInfo = result.Diagnostics?.FirstOrDefault(d => d.Id == "CS8304");
        if (versionInfo is null)
        {
            return;
        }
        var message = versionInfo.GetMessage();
        Console.WriteLine(message);
        VersionString = message;
    }

    private async Task EnsureInitialized()
    {
        if (IsInitialized)
        {
            return;
        }

        bool _isInitializing;
        lock (initializeSyncObject)
        {
            _isInitializing = isInitializing;
        }

        if (_isInitializing)
        {
            while (!IsInitialized)
            {
                await Task.Delay(50);
            }
            return;
        }
        await InitializeAsync();
    }

    /// <summary>
    /// C#コードをコンパイルします。
    /// </summary>
    /// <param name="code">コンパイルするC#コード</param>
    /// <returns></returns>
    public async Task<CompileResult> CompileAsync(string code)
    {
        if (code is null)
        {
            throw new ArgumentNullException(nameof(code));
        }
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
        return await CompileAsync(syntaxTree);
    }

    private Task<CompileResult> CompileAsync(string code, bool skipInitialize)
    {
        if (code is null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
        return CompileAsync(syntaxTree, skipInitialize);
    }

    private async Task<CompileResult> CompileAsync(SyntaxTree syntaxTree, bool skipInitialize = false)
    {
        if (metadataReferences is null || injectCode is null)
        {
            throw new InvalidOperationException("まず初期化処理を行ってからコンパイルを実行する必要があります。");
        }

        if (!skipInitialize)
        {
            await EnsureInitialized();
        }
        var compileOption = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var compile = CSharpCompilation.Create("__HogeAssembly", new[] { syntaxTree, injectCode }, metadataReferences, compileOption);

        Microsoft.CodeAnalysis.Emit.EmitResult? emitResult = default;
        var memoryStream = new MemoryStream();
        try
        {
            for (var i = 0; i < 3; i++)
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

            if (emitResult?.Success ?? false)
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                Assembly asm = AssemblyLoadContext.Default.LoadFromStream(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var bin = memoryStream.ToArray();

                if (TryGetMainMethod(asm, out MethodInfo? methodInfo))
                {
                    return new CompileResult(true, emitResult.Diagnostics, bin, methodInfo);
                }
                else
                {
                    return new CompileResult(false, emitResult.Diagnostics, bin, null);
                }
            }
            return new CompileResult(false, emitResult?.Diagnostics, null, null);
        }
        finally
        {
            memoryStream.Dispose();
        }
    }


    internal static bool TryGetMainMethod(Assembly asm, out MethodInfo? methodInfo)
    {
        MethodInfo[]? mainMethod = asm.GetTypes().Select(x => new[] { x.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static), x.GetMethod("Main", BindingFlags.Public | BindingFlags.Static) }).SelectMany(x => x).Where(x => x is not null).ToArray()!;
        if (mainMethod?.Length != 1)
        {
            methodInfo = null;
            return false;
        }
        else
        {
            methodInfo = mainMethod[0];
            return true;
        }
    }
}

/// <summary>
/// コンパイル結果を表現します。
/// </summary>
internal class CompileResult
{
    /// <summary>
    /// <see cref="CompileResult"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="isSuccessed">コンパイルが成功したかどうか。</param>
    /// <param name="diagnostics">コンパイラからのメッセージ。</param>
    /// <param name="assembly">コンパイルの結果得られたアセンブリ。コンパイルが失敗した場合、<c>null</c> にできます。</param>
    /// <param name="mainMethod">アプリケーションの <c>Main</c> メソッド。コンパイルが失敗した場合、<c>null</c> にできます。</param>
    public CompileResult(bool isSuccessed, IEnumerable<Diagnostic>? diagnostics, byte[] assembly, MethodInfo? mainMethod)
    {
        if (isSuccessed)
        {
            IsSuccessed = isSuccessed;
            Diagnostics = diagnostics;
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            MainMethod = mainMethod ?? throw new ArgumentNullException(nameof(mainMethod));
        }
        else
        {
            IsSuccessed = isSuccessed;
            Diagnostics = diagnostics;
            Assembly = assembly;
            MainMethod = mainMethod;
        }
    }

    /// <summary>
    /// コンパイルが成功したかどうかを表す値を取得します。
    /// </summary>
    public bool IsSuccessed { get; }

    /// <summary>
    /// コンパイラからのメッセージを取得します。
    /// </summary>
    public IEnumerable<Diagnostic>? Diagnostics { get; }

    /// <summary>
    /// コンパイルの結果得られたアセンブリを取得します。コンパイルが成功しなかった場合、<c>null</c> が返されます。
    /// </summary>
    public byte[] Assembly { get; }

    /// <summary>
    /// アプリケーションの <c>Main</c> メソッドを取得します。
    /// </summary>
    public MethodInfo? MainMethod { get; }
}