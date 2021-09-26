using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CodeRunner
{
    /// <summary>
    /// ネットワーク経由でアセンブリを読み込みます。
    /// </summary>
    public class NetworkAssemblyLoader
    {
        private readonly HttpClient httpClient;
        private readonly NetworkAssemblyInfoProvider networkAssemblyInfoProvider = new NetworkAssemblyInfoProvider();

        public NetworkAssemblyLoader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<MetadataReference>> LoadAsync()
        {
            List<MetadataReference> metadataReferences = new List<MetadataReference>();
            foreach (var asmInfo in networkAssemblyInfoProvider.AsmInfo)
            {
                var url = asmInfo.Value;
                var stream = await httpClient.GetStreamAsync(url);
                metadataReferences.Add(MetadataReference.CreateFromStream(stream));
            }
            return metadataReferences;
        }
    }

    public class NetworkAssemblyInfoProvider
    {
        private readonly Dictionary<string, string> asmInfo = new()
        {
            { "System.Private.CoreLib.dll", @"https://2427dkusiro.github.io/BlazorWebAssemblyWeatherForecast/_framework/System.Private.CoreLib.dll" }
        };

        public Dictionary<string, string> AsmInfo { get => asmInfo; }
    }
}
