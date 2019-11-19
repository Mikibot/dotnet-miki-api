using Newtonsoft.Json;

namespace Miki.API.Images.Models
{
    public class UploadResponse
    {
        [JsonProperty("File")]
        public string File { get; internal set; }
    }
}
