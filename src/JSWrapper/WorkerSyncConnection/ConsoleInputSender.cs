using Microsoft.JSInterop;

namespace JSWrapper.WorkerSyncConnection
{
    public class ConsoleInputSender
    {
        private IJSObjectReference module;

        public ConsoleInputSender()
        {

        }

        public async Task InitializeAsync(IJSRuntime jSRuntime)
        {
            module = await jSRuntime.InvokeAsync<IJSObjectReference>("import", SettingProvider.JSDirPath + "UIMessageSender.js");
        }

        public async Task SendInput(string input, string guid)
        {
            await module.InvokeVoidAsync("SendMessage", input, guid, "Input");
        }
    }
}
