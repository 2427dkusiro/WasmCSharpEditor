using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WasmCsTest.Codes
{
    public class CodeMirrorWrapper
    {
        private Guid guid;
        private IJSObjectReference module;

        public event EventHandler OnChange
        {
            add => EventWrapper.AddHandler(guid, value);
            remove => EventWrapper.RemoveHandler(guid, value);
        }

        private CodeMirrorWrapper()
        {

        }

        public static async Task<CodeMirrorWrapper> CreateInstanceAsync(IJSRuntime jSRuntime)
        {
            CodeMirrorWrapper codeMirrorWrapper = new CodeMirrorWrapper();
            codeMirrorWrapper.guid = Guid.NewGuid();
            codeMirrorWrapper.module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/CodeEditorHandler.js");
            await codeMirrorWrapper.InitializeEvent();

            return codeMirrorWrapper;
        }

        private async Task InitializeEvent()
        {
            await module.InvokeVoidAsync("EnableEventRaising", guid.ToString());
        }

        public async Task SetToTextArea(string textAreaId)
        {
            await module.InvokeVoidAsync("InitializeCodeEditor", textAreaId);
        }

        public async Task<string> GetCodeText()
        {
            return await module.InvokeAsync<string>("Save", Array.Empty<object>());
        }
    }
}
