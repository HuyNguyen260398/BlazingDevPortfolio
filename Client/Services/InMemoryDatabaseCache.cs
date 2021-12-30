using Shared.Models;

namespace Client.Services;

internal sealed class InMemoryDatabaseCache
{
    private readonly HttpClient _httpClient;

    public InMemoryDatabaseCache(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private List<Category> categories = null;

    internal List<Category> Categories
    {
        get { return categories; }
        set 
        { 
            categories = value;
            NotifyCategoriesDataChanged();
        }
    }

    internal event Action OnCategoriesDataChanged;

    private void NotifyCategoriesDataChanged() => OnCategoriesDataChanged?.Invoke();
}
