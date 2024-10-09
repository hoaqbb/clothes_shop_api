using clothes_shop_api.Interfaces;

namespace clothes_shop_api.Services
{
    public class FileService : IFileService
    {
        public Task UploadFile(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public Task UploadMultipleFiles(List<IFormFile> files)
        {
            throw new NotImplementedException();
        }
    }
}
