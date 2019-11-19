using Newtonsoft.Json;

namespace Miki.API.Images.Models
{
    internal class PostImage
    {
        [JsonProperty("Tags")]
        public string[] Tags { get; internal set; }
        [JsonProperty("Data")]
        public string Data { get; internal set; }
    }
}
