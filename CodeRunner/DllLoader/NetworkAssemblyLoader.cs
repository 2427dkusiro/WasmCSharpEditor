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
        private readonly string indexPath;

        public NetworkAssemblyLoader(HttpClient httpClient, string indexPath)
        {
            this.httpClient = httpClient;
            this.indexPath = indexPath;
        }

        public async Task<IEnumerable<MetadataReference>> LoadAsync()
        {
            var info = await httpClient.GetFromJsonAsync<LoadInfo>(indexPath);
            var str = LoadInfo.DirRelativePath;
            var data = await Task.WhenAll(info.DllLoadInfos.Select(async dllInfo =>
            {
                var path = $"{str}{dllInfo.Name}";
                var stream = await httpClient.GetStreamAsync(path);
                return MetadataReference.CreateFromStream(stream);
            }));
            return data;
        }
    }
}
