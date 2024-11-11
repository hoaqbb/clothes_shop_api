using clothes_shop_api.Helpers;
using clothes_shop_api.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace clothes_shop_api.Services
{
    public class FileService : IFileService
    {
        private readonly Cloudinary _cloudinary;
        public FileService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    AssetFolder = "cloths-shop"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);

            }

            return uploadResult;
        }

        public async Task<List<ImageUploadResult>> AddMultipleImageAsync(IFormFile[] files)
        {
            var results = new List<ImageUploadResult>();
            if(files.Length > 0)
            {
                foreach (var file in files)
                {
                    results.Add(await AddImageAsync(file));
                }
            }

            return results;
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}
