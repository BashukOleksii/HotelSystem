using Microsoft.AspNetCore.Http;

namespace lab_01.Services.Interfaces
{
    public interface IImageStorageService
    {
        Task<IReadOnlyList<string>> SaveImagesAsync(
            IEnumerable<IFormFile>? files,
            string folder,
            int maxFiles
        );

        Task DeleteImagesAsync(
            IEnumerable<string> urls
        );
    }
}