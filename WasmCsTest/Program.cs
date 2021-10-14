using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using WasmCsTest.WorkerConnection;

namespace WasmCsTest
{
    /// <summary>
    /// Web�A�v���P�[�V�����̃G���g���|�C���g���i�[���ꂽ�N���X�ł��B
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Web�A�v���P�[�V�����̃G���g���|�C���g�ł��B
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton<CompileQueueService>();

            await builder.Build().RunAsync();
        }
    }
}
