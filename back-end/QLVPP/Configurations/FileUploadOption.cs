public class FileUploadOptions
{
    public string BasePath { get; set; } = default!;
    public Dictionary<string, string> Folders { get; set; } = new();
}
