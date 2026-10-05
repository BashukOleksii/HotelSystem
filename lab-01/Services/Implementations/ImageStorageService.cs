using lab_01.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace lab_01.Services.Implementations
{
    public class ImageStorageService
        : IImageStorageService
    {
        private const long MaxFileSize =
            5 * 1024 * 1024;

        private static readonly Dictionary<
            string,
            string[]> AllowedTypes =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [".jpg"] =
                [
                    "image/jpeg"
                ],

                [".jpeg"] =
                [
                    "image/jpeg"
                ],

                [".png"] =
                [
                    "image/png"
                ],

                [".webp"] =
                [
                    "image/webp"
                ]
            };

        private readonly IWebHostEnvironment _environment;

        public ImageStorageService(
            IWebHostEnvironment environment)
        {
            _environment =
                environment;
        }

        public async Task<IReadOnlyList<string>>
            SaveImagesAsync(
                IEnumerable<IFormFile>? files,
                string folder,
                int maxFiles)
        {
            if (files is null)
            {
                return [];
            }

            List<IFormFile> images =
                files
                    .Where(file =>
                        file is not null &&
                        file.Length > 0)
                    .ToList();

            if (images.Count == 0)
            {
                return [];
            }

            if (images.Count > maxFiles)
            {
                throw new ArgumentException(
                    $"Можна завантажити не більше {maxFiles} фотографій."
                );
            }

            string webRoot =
                GetWebRoot();

            string directory =
                Path.Combine(
                    webRoot,
                    "uploads",
                    folder
                );

            Directory.CreateDirectory(
                directory
            );

            List<string> savedUrls = [];

            try
            {
                foreach (IFormFile file in images)
                {
                    ValidateFile(
                        file
                    );

                    string extension =
                        Path.GetExtension(
                            file.FileName
                        )
                        .ToLowerInvariant();

                    bool signatureIsValid =
                        await HasValidImageSignatureAsync(
                            file,
                            extension
                        );

                    if (!signatureIsValid)
                    {
                        throw new ArgumentException(
                            $"Файл '{file.FileName}' не є коректним зображенням."
                        );
                    }

                    string fileName =
                        $"{Guid.NewGuid():N}{extension}";

                    string fullPath =
                        Path.Combine(
                            directory,
                            fileName
                        );

                    await using FileStream stream =
                        new FileStream(
                            fullPath,
                            FileMode.CreateNew
                        );

                    await file.CopyToAsync(
                        stream
                    );

                    savedUrls.Add(
                        $"/uploads/{folder}/{fileName}"
                    );
                }

                return savedUrls;
            }
            catch
            {
                await DeleteImagesAsync(
                    savedUrls
                );

                throw;
            }
        }

        public Task DeleteImagesAsync(
            IEnumerable<string> urls)
        {
            string webRoot =
                GetWebRoot();

            string uploadsRoot =
                Path.GetFullPath(
                    Path.Combine(
                        webRoot,
                        "uploads"
                    )
                );

            foreach (string url in urls)
            {
                if (string.IsNullOrWhiteSpace(
                    url))
                {
                    continue;
                }

                if (!url.StartsWith(
                    "/uploads/",
                    StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string relativePath =
                    url
                        .TrimStart('/')
                        .Replace(
                            '/',
                            Path.DirectorySeparatorChar
                        );

                string fullPath =
                    Path.GetFullPath(
                        Path.Combine(
                            webRoot,
                            relativePath
                        )
                    );

                if (!fullPath.StartsWith(
                    uploadsRoot,
                    StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (File.Exists(
                    fullPath))
                {
                    File.Delete(
                        fullPath
                    );
                }
            }

            return Task.CompletedTask;
        }

        private void ValidateFile(
            IFormFile file)
        {
            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException(
                    $"Файл '{file.FileName}' перевищує максимальний розмір 5 МБ."
                );
            }

            string extension =
                Path.GetExtension(
                    file.FileName
                )
                .ToLowerInvariant();

            if (!AllowedTypes.TryGetValue(
                extension,
                out string[]? allowedContentTypes))
            {
                throw new ArgumentException(
                    "Дозволені формати: JPG, JPEG, PNG, WEBP."
                );
            }

            if (!allowedContentTypes.Contains(
                file.ContentType,
                StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"Некоректний тип файлу '{file.FileName}'."
                );
            }
        }

        private static async Task<bool>
            HasValidImageSignatureAsync(
                IFormFile file,
                string extension)
        {
            byte[] header =
                new byte[12];

            await using Stream stream =
                file.OpenReadStream();

            int bytesRead =
                await stream.ReadAsync(
                    header.AsMemory(
                        0,
                        header.Length
                    )
                );

            if (extension is ".jpg" or ".jpeg")
            {
                return bytesRead >= 3 &&
                       header[0] == 0xFF &&
                       header[1] == 0xD8 &&
                       header[2] == 0xFF;
            }

            if (extension == ".png")
            {
                byte[] pngSignature =
                [
                    0x89,
                    0x50,
                    0x4E,
                    0x47,
                    0x0D,
                    0x0A,
                    0x1A,
                    0x0A
                ];

                return bytesRead >= 8 &&
                       header
                           .Take(8)
                           .SequenceEqual(
                               pngSignature
                           );
            }

            if (extension == ".webp")
            {
                return bytesRead >= 12 &&
                       header[0] == (byte)'R' &&
                       header[1] == (byte)'I' &&
                       header[2] == (byte)'F' &&
                       header[3] == (byte)'F' &&
                       header[8] == (byte)'W' &&
                       header[9] == (byte)'E' &&
                       header[10] == (byte)'B' &&
                       header[11] == (byte)'P';
            }

            return false;
        }

        private string GetWebRoot()
        {
            return _environment.WebRootPath
                   ?? Path.Combine(
                       _environment.ContentRootPath,
                       "wwwroot"
                   );
        }
    }
}