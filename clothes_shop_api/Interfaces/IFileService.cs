using CloudinaryDotNet.Actions;

namespace clothes_shop_api.Interfaces
{
    public interface IFileService
    {
        Task<ImageUploadResult> AddImageAsync(IFormFile file);
        Task<List<ImageUploadResult>> AddMultipleImageAsync(IFormFile[] files);
        Task<DeletionResult> DeleteImageAsync(string publicId);
    }
}
