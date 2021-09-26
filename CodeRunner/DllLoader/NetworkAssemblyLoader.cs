using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<MetadataReference> metadataReferences = new List<MetadataReference>();

            var info = await httpClient.GetFromJsonAsync<LoadInfo>(indexPath);
            var str = LoadInfo.DirRelativePath;
            foreach (var dll in info.DllLoadInfos)
            {
                var path = $"{str}{dll.Name}";
                using (var stream = await httpClient.GetStreamAsync(path))
                {
                    metadataReferences.Add(MetadataReference.CreateFromStream(stream));
                }
            }
            stopwatch.Stop();
            return metadataReferences;
        }
    }
}
