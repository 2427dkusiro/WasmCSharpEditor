using BlazorWorker.Extensions.JSRuntime;
using BlazorWorker.WorkerCore;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner
{
    /// <summary>
    /// <see cref="CodeCompileService"/> の起動を簡略化します。
    /// </summary>
    /// <remarks>
    /// BlazorWorker.Demo.IoCExample.MyIndexDBServiceStartupあたりを参考に実装したがいまいちよくわかってない
    /// </remarks>
    public class CodeCompileServiceStartup
    {
        private readonly HttpClient httpClient;
        private readonly IServiceProvider serviceProvider;
        private readonly IWorkerMessageService workerMessageService;

        /// <summary>
        /// <see cref="CodeCompileServiceStartup"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="httpClient">有効な <see cref="HttpClient"/>。</param>
        /// <param name="workerMessageService">親から渡される <see cref="IWorkerMessageService"/>。</param>
        public CodeCompileServiceStartup(HttpClient httpClient, IWorkerMessageService workerMessageService)
        {
            this.httpClient = httpClient;
            this.workerMessageService = workerMessageService;
            serviceProvider = ServiceCollectionHelper.BuildServiceProviderFromMethod(Configure);
        }

        /// <summary>
        /// サービスを取得します。
        /// </summary>
        /// <typeparam name="T">取得するサービスの型。</typeparam>
        /// <returns></returns>
        public T? Resolve<T>()
        {
            return serviceProvider.GetService<T>();
        }

        /// <summary>
        /// <see cref="CodeCompileService"/> を適切に設定して起動します。
        /// </summary>
        /// <param name="services"></param>
        public void Configure(IServiceCollection services)
        {
            services
                    .AddBlazorWorkerJsRuntime()
                    .AddSingleton(httpClient)
                    .AddSingleton<CodeCompileService>()
                    .AddSingleton(workerMessageService);
        }
    }

    /// <summary>
    /// <see cref="ServiceCollection"/> への操作を簡略化します。
    /// </summary>
    public static class ServiceCollectionHelper
    {
        /// <summary>
        /// サービスの初期設定をするメソッドをカプセル化します。
        /// </summary>
        /// <param name="services"></param>
        public delegate void Configure(IServiceCollection services);

        /// <summary>
        /// 初期設定をするメソッドをもとに <see cref="IServiceProvider "/> を初期化します。
        /// </summary>
        /// <param name="configureMethod"></param>
        /// <returns></returns>
        public static IServiceProvider BuildServiceProviderFromMethod(Configure configureMethod)
        {
            var serviceCollection = new ServiceCollection();
            configureMethod(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        }
    }
}
