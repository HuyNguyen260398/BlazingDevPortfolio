using Client.Static;
using Shared.Models;
using System.Net.Http.Json;

namespace Client.Services;

internal sealed class InMemoryDatabaseCache
{
    private readonly HttpClient _httpClient;

    public InMemoryDatabaseCache(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    #region Categories

    private List<Category> _categories = null;

    internal List<Category> Categories
    {
        get { return _categories; }
        set
        {
            _categories = value;
            NotifyCategoriesDataChanged();
        }
    }

    private bool _gettingCategoriesFromDatabaseAndCaching = false;

    internal async Task<Category> GetCategoryById(int categoryId, bool withPosts)
    {
        if (_categories == null)
            await GetCategoriesFromDatabaseAndCache(withPosts);

        Category categoryToReturn = _categories.FirstOrDefault(c => c.CategoryId == categoryId);

        if (categoryToReturn.Posts == null && withPosts)
            categoryToReturn = await _httpClient.GetFromJsonAsync<Category>($"{ApiEndpoints.s_categoriesWithPosts}/{categoryToReturn.CategoryId}");

        return categoryToReturn;
    }

    internal async Task<Category> GetCategoryByName(string categoryName, bool withPosts, bool nameToLowerFromUrl)
    {
        if (_categories == null)
            await GetCategoriesFromDatabaseAndCache(withPosts);

        Category categoryToReturn = null;

        if (nameToLowerFromUrl)
            categoryToReturn = _categories.FirstOrDefault(c => c.Name.ToLowerInvariant() == categoryName);
        else
            categoryToReturn = _categories.FirstOrDefault(c => c.Name == categoryName);

        if (categoryToReturn.Posts == null && withPosts)
            categoryToReturn = await _httpClient.GetFromJsonAsync<Category>($"{ApiEndpoints.s_categoriesWithPosts}/{categoryToReturn.CategoryId}");

        return categoryToReturn;
    }

    internal async Task GetCategoriesFromDatabaseAndCache(bool withPosts)
    {
        // Only allow one Get request to run at a time
        if (!_gettingCategoriesFromDatabaseAndCaching)
        {
            _gettingCategoriesFromDatabaseAndCaching = true;
            List<Category> categoriesFromDb = null;

            if (_categories != null)
                _categories = null;

            if (withPosts)
                categoriesFromDb = await _httpClient.GetFromJsonAsync<List<Category>>(ApiEndpoints.s_categoriesWithPosts);
            else
                categoriesFromDb = await _httpClient.GetFromJsonAsync<List<Category>>(ApiEndpoints.s_categories);

            _categories = categoriesFromDb.OrderByDescending(c => c.CategoryId).ToList();

            if (withPosts)
            {
                List<Post> postsFromCategory = new();
                foreach (var category in _categories)
                {
                    if (category.Posts.Count != 0)
                    {
                        foreach (var post in category.Posts)
                        {
                            Category postCategoryWithoutPosts = new()
                            {
                                CategoryId = category.CategoryId,
                                ThumbnailImagePath = category.ThumbnailImagePath,
                                Name = category.Name,
                                Description = category.Description,
                                Posts = null
                            };
                            post.Category = postCategoryWithoutPosts;
                            postsFromCategory.Add(post);
                        }
                    }
                }
                _posts = postsFromCategory.OrderByDescending(p => p.PostId).ToList();
            }

            _categories = await _httpClient.GetFromJsonAsync<List<Category>>(ApiEndpoints.s_categories);
            _gettingCategoriesFromDatabaseAndCaching = false;
        }
    }

    internal event Action OnCategoriesDataChanged;

    private void NotifyCategoriesDataChanged() => OnCategoriesDataChanged?.Invoke();

    #endregion

    #region Posts

    private List<Post> _posts = null;

    internal List<Post> Posts
    {
        get { return _posts; }
        set
        {
            _posts = value;
            NotifyPostsDataChanged();
        }
    }

    internal async Task<Post> GetPostById(int postId)
    {
        if (_posts == null)
            await GetPostsFromDbAndCache();

        return _posts.FirstOrDefault(p => p.PostId == postId);
    }

    internal async Task<PostDto> GetPostDtoById(int postId) => await _httpClient.GetFromJsonAsync<PostDto>($"{ApiEndpoints.s_postsDto}/{postId}");

    private bool _gettingPostFromDbAndCaching = false;

    internal async Task GetPostsFromDbAndCache()
    {
        // Only allow one Get to run at a time
        if (!_gettingCategoriesFromDatabaseAndCaching)
        {
            _gettingCategoriesFromDatabaseAndCaching = true;

            if (_posts != null)
                _posts = null;

            List<Post> postsFromDb = await _httpClient.GetFromJsonAsync<List<Post>>(ApiEndpoints.s_posts);
            _posts = postsFromDb.OrderByDescending(p => p.PostId).ToList();
            _gettingCategoriesFromDatabaseAndCaching = false;
        }
    }

    internal event Action OnPostsDataChanged;

    private void NotifyPostsDataChanged() => OnPostsDataChanged?.Invoke();

    #endregion
}
