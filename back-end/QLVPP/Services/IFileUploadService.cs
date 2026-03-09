public interface IFileUploadService
{
    Task<string> UploadAsync(IFormFile file, UploadFolder folderType);
}
