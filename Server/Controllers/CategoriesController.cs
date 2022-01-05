using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    #region  CRUD Operations

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var categories = await _db.Categories.ToListAsync();
        return Ok(categories);
    } 

    // api/categories/withposts
    [HttpGet("withposts")]
    public async Task<IActionResult> GetWithPosts()
    {
        List<Category> categories = await _db.Categories
                                             .Include(c => c.Posts)
                                             .ToListAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        Category category = await GetCategoryByCategoryId(id, false);
        return Ok(category);
    }

    [HttpGet("withposts/{id}")]
    public async Task<IActionResult> GetWithPost(int id)
    {
        Category category = await GetCategoryByCategoryId(id, true);
        return Ok(category);
    }

    #endregion

    #region Utility Methods

    [NonAction]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<bool> PersistChangesToDatabase()
    {
        int amountOfChanges = await _db.SaveChangesAsync();

        return amountOfChanges > 0;
    }

    [NonAction]
    [ApiExplorerSettings(IgnoreApi = true)]
    private async Task<Category> GetCategoryByCategoryId(int categoryId, bool withPosts)
    {
        Category categoryToGet = null;

        if (withPosts)
        {
            categoryToGet = await _db.Categories
                                     .Include(c => c.Posts)
                                     .FirstAsync(c => c.CategoryId == categoryId);
        }
        else
        {
            categoryToGet = await _db.Categories
                                     .FirstAsync(c => c.CategoryId == categoryId);
        }
        return categoryToGet;
    }
    
    #endregion
}
