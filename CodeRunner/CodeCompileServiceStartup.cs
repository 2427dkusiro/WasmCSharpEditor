using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BlazorWorker.WorkerCore;
using BlazorWorker.Extensions.JSRuntime;

using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace CodeRunner
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// BlazorWorker.Demo.IoCExample.MyIndexDBServiceStartupあたりを参考に実装したがいまいちよくわかってない
    /// </remarks>
    public class CodeCompileServiceStartup
    {
        private readonly HttpClient httpClient;
        private readonly IServiceProvider serviceProvider;
        private readonly IWorkerMessageService workerMessageService;

        public CodeCompileServiceStartup(HttpClient httpClient, IWorkerMessageService workerMessageService)
        {
            this.httpClient = httpClient;
            this.workerMessageService = workerMessageService;
            serviceProvider = ServiceCollectionHelper.BuildServiceProviderFromMethod(Configure);
        }

        public T Resolve<T>() => serviceProvider.GetService<T>();

        public void Configure(IServiceCollection services)
        {
            services
                    .AddBlazorWorkerJsRuntime()
                    .AddSingleton(httpClient)
                    .AddSingleton<CodeCompileService>()
                    .AddSingleton(workerMessageService);
        }
    }

    public static class ServiceCollectionHelper
    {
        public delegate void Configure(IServiceCollection services);

        public static IServiceProvider BuildServiceProviderFromMethod(Configure configureMethod)
        {
            var serviceCollection = new ServiceCollection();
            configureMethod(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        }
    }
}
