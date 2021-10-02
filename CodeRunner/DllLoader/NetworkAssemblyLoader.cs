using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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

        public NetworkAssemblyLoader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

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
