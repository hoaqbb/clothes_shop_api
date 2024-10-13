using CloudinaryDotNet.Actions;

namespace clothes_shop_api.Interfaces
{
    public interface IFileService
    {
        Task UploadFile(IFormFile file);
        Task UploadMultipleFiles(List<IFormFile> files);
        Task<ImageUploadResult> AddImageAsync(IFormFile file);
        Task<DeletionResult> DeleteImageAsync(string publicId);
    }
}
