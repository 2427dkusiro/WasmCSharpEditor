using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner.DllLoader
{
    /// <summary>
    /// ネットワーク経由でアセンブリを読み込みます。
    /// </summary>
    public class NetworkAssemblyLoader
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// <see cref="NetworkAssemblyLoader"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="httpClient">有効な <see cref="HttpClient"/>。</param>
        public NetworkAssemblyLoader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// アセンブリをロードします。
        /// </summary>
        /// <returns>ロードしたアセンブリから得た型情報。</returns>
        public async Task<IEnumerable<MetadataReference>> LoadAsync()
        {
            IEnumerable<string> paths = await DllInfoProvider.GetDllPaths(httpClient, System.Globalization.CultureInfo.CurrentUICulture);
            return await Task.WhenAll(paths.Select(async path =>
            {
                System.IO.Stream stream = await httpClient.GetStreamAsync(path);
                return MetadataReference.CreateFromStream(stream);
            }));
        }
    }
}
