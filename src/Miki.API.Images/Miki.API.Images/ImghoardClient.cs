using Miki.API.Images.Exceptions;
using Miki.API.Images.Models;
using Miki.Utils.Imaging.Headers;
using Miki.Utils.Imaging.Headers.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Miki.API.Images
{
    public class ImghoardClient : IImghoardClient
    {
        private HttpClient apiClient;
        private readonly Config config;
        private const int Mb = 1000000;
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public ImghoardClient() : this(Config.Default()) { }

        public ImghoardClient(Config config)
        {
            this.config = config;

            this.apiClient = new HttpClient();
            this.apiClient.DefaultRequestHeaders.Add("x-miki-tenancy", config.Tenancy);
            this.apiClient.DefaultRequestHeaders.Add("User-Agent", config.UserAgent);
        }

        public string GetEndpoint()
            => config.Endpoint;

        /// <summary>
        /// Gets the first page of results given an array of Tags to find
        /// </summary>
        /// <param name="tags">Tags to search for</param>
        /// <returns>A readonly list of images found with the Tags entered</returns>
        public async Task<ImagesResponse> GetImagesAsync(params string[] tags)
            => await GetImagesAsync(0, tags);

        /// <summary>
        /// Gets the given page of results given an array of Tags to find
        /// </summary>
        /// <param name="tags">Tags to search for</param>
        /// <returns>A readonly list of images found with the Tags entered</returns>
        public async Task<ImagesResponse> GetImagesAsync(int page = 0, params string[] tags)
        {
            List<string> args = new List<string>();

            if (page > 0)
            {
                args.Add($"page{page}");
            }

            if (tags.Any())
            {
                args.Add(string.Join("+", tags));
            }

            string url = "";

            if (args.Any())
                url = $"?{string.Join("&", args)}";

            var response = await apiClient.GetAsync(config.Endpoint + url);

            if (response.IsSuccessStatusCode)
            {
                return new ImagesResponse(this, JsonConvert.DeserializeObject<IReadOnlyList<Image>>(await response.Content.ReadAsStringAsync()), tags, page);
            }

            throw new ResponseException("Response was not successfull; Reason: \"" + response.ReasonPhrase + "\"");
        }

        /// <summary>
        /// Get an image with a given Id
        /// </summary>
        /// <param name="id">The snowflake Id of the Image to get</param>
        /// <returns>The image with the given snowflake</returns>
        public async Task<Image> GetImageAsync(ulong id)
        {
            var response = await apiClient.GetAsync(config.Endpoint + $"/{id}");

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Image>(await response.Content.ReadAsStringAsync());
            }

            throw new ResponseException(response.ReasonPhrase);
        }

        /// <summary>
        /// Posts a new image to the Imghoard instance
        /// </summary>
        /// <param name="image">The image stream to upload</param>
        /// <param name="tags">The tags of the image being uploaded</param>
        /// <returns>The url of the uploaded image or null on failure</returns>
        public async Task<string> PostImageAsync(Stream image, params string[] tags)
        {
            byte[] bytes;

            using (var mStream = new MemoryStream())
            {
                await image.CopyToAsync(mStream);
                bytes = mStream.ToArray();
            }
            image.Position = 0;

            return await PostImageAsync(bytes, tags);
        }

        /// <summary>
        /// Posts a new image to the Imghoard instance
        /// </summary>
        /// <param name="bytes">The raw bytes of the image to upload</param>
        /// <param name="tags">The tags of the image being uploaded</param>
        /// <returns>The url of the uploaded image or null on failure</returns>
        public async Task<string> PostImageAsync(Memory<byte> bytes, params string[] tags)
        {
            if (bytes.Length >= Mb && !config.Experimental)
            {
                throw new NotSupportedException("In order to upload images larger than 1MB you need to enable experimental features in the config");
            }

            var check = IsSupported(bytes.Span);

            if (!check.Supported)
            {
                throw new NotSupportedException("You have given an incorrect image format, currently supported formats are: png, jpeg, gif");
            }

            if (bytes.Length < Mb)
            {
                var body = JsonConvert.SerializeObject(
                        new PostImage
                        {
                            Data = $"data:image/{check.Prefix};base64,{Convert.ToBase64String(bytes.Span)}",
                            Tags = tags
                        },
                        serializerSettings
                );

                var response = await apiClient.PostAsync(config.Endpoint, new StringContent(body));

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<UploadResponse>(await response.Content.ReadAsStringAsync()).File;
                }

                throw new ResponseException("Response was not successfull; Reason: \"" + response.ReasonPhrase + "\"");
            }
            else
            {
                var body = new MultipartFormDataContent
                {
                    { new StringContent($"image/{check.Prefix}"), "data-type" },
                    { new ByteArrayContent(bytes.Span.ToArray()), "data" },
                    { new StringContent(string.Join(",", tags)), "tags" }
                };

                var response = await apiClient.PostAsync(config.Endpoint, body);

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<UploadResponse>(await response.Content.ReadAsStringAsync()).File;
                }

                throw new ResponseException("Response was not successfull; Reason: \"" + response.ReasonPhrase + "\"");
            }
        }

        private SupportedImage IsSupported(Span<byte> image)
        {
            if (ImageHeaders.Validate(image, ImageType.Png))
            {
                return new SupportedImage(true, "png");
            }
            if (ImageHeaders.Validate(image, ImageType.Jpeg))
            {
                return new SupportedImage(true, "jpeg");
            }
            if (ImageHeaders.Validate(image, ImageType.Gif89a)
                || ImageHeaders.Validate(image, ImageType.Gif87a))
            {
                return new SupportedImage(true, "gif");
            }
            return new SupportedImage(false, null);
        }
    }
}
