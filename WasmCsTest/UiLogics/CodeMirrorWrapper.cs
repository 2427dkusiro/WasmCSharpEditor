using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using WasmCsTest.WorkerConnection;

namespace WasmCsTest.UiLogics
{
    /// <summary>
    /// CodeMirrorの簡易的なC#ラッパーを提供します。
    /// </summary>
    public class CodeMirrorWrapper
    {
        private Guid guid;
        private IJSObjectReference? module;

        /// <summary>
        /// コードが変更されたときに発生するイベント。
        /// </summary>
        public event EventHandler OnChange
        {
            add => EventWrapper.AddHandler(guid, value);
            remove => EventWrapper.RemoveHandler(guid, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// このクラスの初期化プロセスを非同期で実行するために、コンストラクタではなく <see cref="CreateInstanceAsync(IJSRuntime)"/> を呼び出します。
        /// </remarks>
        private CodeMirrorWrapper()
        { }

        /// <summary>
        /// <see cref="CodeMirrorWrapper"/> クラスの新しいインスタンスを取得します。
        /// </summary>
        /// <param name="jSRuntime"></param>
        /// <returns></returns>
        public static async Task<CodeMirrorWrapper> CreateInstanceAsync(IJSRuntime jSRuntime)
        {
            var codeMirrorWrapper = new CodeMirrorWrapper
            {
                guid = Guid.NewGuid(),
                module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/CodeEditorHandler.js")
            };
            await codeMirrorWrapper.InitializeEvent();

            return codeMirrorWrapper;
        }

        private async Task InitializeEvent()
        {
            if (module is null)
            {
                throw new InvalidOperationException("モジュールがインポートされていません。");
            }
            await module.InvokeVoidAsync("EnableEventRaising", guid.ToString());
        }

        /// <summary>
        /// HTMLのtextArea要素にコードエディタをアタッチします。
        /// </summary>
        /// <param name="textAreaId">テキストエディタに設定したID属性の値。</param>
        /// <param name="codeMirrorOption">構成オプション。デフォルト設定を使用する場合は、<c>null</c> にできます。</param>
        /// <returns></returns>
        public async Task SetToTextArea(string textAreaId, CodeMirrorOption? codeMirrorOption = null)
        {
            if (module is null)
            {
                throw new InvalidOperationException("モジュールがインポートされていません。");
            }
            await module.InvokeVoidAsync("InitializeCodeEditor", textAreaId, codeMirrorOption ?? new CodeMirrorOption());
        }

        /// <summary>
        /// 現在のコードエディタのテキストを取得します。
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetCodeText()
        {
            if (module is null)
            {
                throw new InvalidOperationException("モジュールがインポートされていません。");
            }
            return await module.InvokeAsync<string>("Save");
        }
    }

    /// <summary>
    /// CodeMirror の構成オプションを表現します。
    /// </summary>
    public class CodeMirrorOption
    {
        /// <summary>
        /// 初期状態でのコードを取得または設定します。
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// インデントのスペース数を取得または設定します。
        /// </summary>
        public int IndentUnit { get; set; } = 2;
    }
}
