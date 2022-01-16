namespace Client.Static;

internal static class ApiEndpoints
{
#if DEBUG
    internal const string ServerBaseUrl = "https://localhost:7054";
#else
    internal const string ServerBaseUrl = "";
#endif
    internal readonly static string s_categories = $"{ServerBaseUrl}/api/categories";
    internal readonly static string s_imgUpload = $"{ServerBaseUrl}/api/imageupload";
}
