using BlazorTask;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using WasmCSharpEditor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
WasmCSharpEditor.WorkerConnections.CompileQueueService compileQueueService = new();

builder.Services.AddSingleton<WasmCSharpEditor.WorkerConnections.CompileQueueService>(compileQueueService);
builder.Services.AddWorkerService(config => config
                    .ResolveResourcesFromBootJson(config.HttpClient));
                    // .FetchBrotliResources("decode.min.js"));

WebAssemblyHost? host = builder.Build();

await Task.WhenAll(host.InitializeWorkerService(), host.RunAsync());
