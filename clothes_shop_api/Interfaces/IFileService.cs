namespace clothes_shop_api.Interfaces
{
    public interface IFileService
    {
        Task UploadFile(IFormFile file);
        Task UploadMultipleFiles(List<IFormFile> files);
    }
}
