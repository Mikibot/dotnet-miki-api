using Miki.API.Images.Models;
using System;
using System.Threading.Tasks;

namespace Miki.API.Images
{
    public interface IImghoardClient
    {
        Task<ImagesResponse> GetImagesAsync(params string[] Tags);
        Task<ImagesResponse> GetImagesAsync(int page = 0, params string[] Tags);
        Task<Image> GetImageAsync(ulong Id);
        Task<string> PostImageAsync(Memory<byte> bytes, params string[] Tags);
    }
}
