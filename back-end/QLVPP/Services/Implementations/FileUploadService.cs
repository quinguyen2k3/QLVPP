using Microsoft.Extensions.Options;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _env;
    private readonly FileUploadOptions _options;

    public FileUploadService(IWebHostEnvironment env, IOptions<FileUploadOptions> options)
    {
        _env = env;
        _options = options.Value;
    }

    public async Task<string> UploadAsync(IFormFile file, UploadFolder folderType)
    {
        var folderKey = folderType.ToString();

        if (!_options.Folders.TryGetValue(folderKey, out var subFolder))
            throw new InvalidOperationException($"Upload folder not configured: {folderKey}");

        var basePath = _options.BasePath;
        if (string.IsNullOrEmpty(basePath))
            throw new InvalidOperationException("BasePath is not configured in FileUploadOptions");

        var rootPath = _env.WebRootPath ?? Directory.GetCurrentDirectory();

        var physicalPath = Path.Combine(rootPath, basePath.Replace("wwwroot/", ""), subFolder);

        Directory.CreateDirectory(physicalPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(physicalPath, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{subFolder}/{fileName}";
    }
}
