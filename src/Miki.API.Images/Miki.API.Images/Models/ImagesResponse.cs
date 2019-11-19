using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.API.Images.Models
{
    public class ImagesResponse
    {
        private readonly ImghoardClient clientInstance;

        public IReadOnlyList<Image> Images { get; private set; }
        public string[] QueryTags { get; private set; }
        public int Page { get; private set; }

        /// <summary>
        /// Creates a new instance of ImagesResponse
        /// </summary>
        /// <param name="client">An instance of the Imghoard Client</param>
        /// <param name="images">The list of images currently gotten</param>
        /// <param name="queryTags">Query tags used to get the images</param>
        /// <param name="page">Current page the response is on</param>
        public ImagesResponse(ImghoardClient client, IEnumerable<Image> images, string[] queryTags, int page)
        {
            this.clientInstance = client;
            this.Images = images.ToList();
            this.QueryTags = queryTags;
            this.Page = page;
        }

        /// <summary>
        /// Get the next page using the same tags as before
        /// </summary>
        /// <returns>The next page of images</returns>
        public async Task<ImagesResponse> GetNextPageAsync() =>
            await clientInstance.GetImagesAsync(Page + 1, QueryTags);
    }
}
