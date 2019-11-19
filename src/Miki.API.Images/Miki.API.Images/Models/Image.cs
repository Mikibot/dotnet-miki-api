using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Miki.API.Images.Tests")]
namespace Miki.API.Images.Models
{
    public class Image
    {
        [JsonProperty("ID")]
        public ulong Id { get; internal set; }
        [JsonProperty("Tags")]
        public string[] Tags { get; internal set; }
        [JsonProperty("URL")]
        public string Url { get; internal set; }
    }
}
