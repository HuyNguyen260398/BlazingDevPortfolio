namespace Shared.Models;

public class UploadedImage
{
    public string NewImageExtension { get; set; }

    // Base64 is basically a string that represent binary
    public string NewImageBase64Content { get; set; }

    public string OldImagePath { get; set; }
}
