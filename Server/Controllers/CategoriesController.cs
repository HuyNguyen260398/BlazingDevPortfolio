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
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CategoriesController(AppDbContext db, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _webHostEnvironment = webHostEnvironment;
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Category categoryToCreate)
    {
        try
        {
            if (categoryToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _db.Categories.AddAsync(categoryToCreate);
            bool changesPersitsToDb = await PersistChangesToDatabase();

            if (!changesPersitsToDb)
            {
                return StatusCode(500, "Error");
            }
            return Created("Create", categoryToCreate);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Category updatedCategory)
    {
        try
        {
            if (id < 1 || updatedCategory == null || id != updatedCategory.CategoryId)
            {
                return BadRequest(ModelState);
            }

            bool isExists = await _db.Categories.AnyAsync(c => c.CategoryId == id);

            if (!isExists)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Categories.Update(updatedCategory);
            bool changesPersitsToDb = await PersistChangesToDatabase();

            if (!changesPersitsToDb)
            {
                return StatusCode(500, "Error");
            }
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id < 1)
            {
                return BadRequest(ModelState);
            }

            bool isExsisted = await _db.Categories.AnyAsync(c => c.CategoryId == id);
            if (!isExsisted)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Category categoryToDelete = await GetCategoryByCategoryId(id, false);
            if (categoryToDelete.ThumbnailImagePath != "uploads/placeholder.jpg")
            {
                string fileName = categoryToDelete.ThumbnailImagePath.Split('/').Last();
                System.IO.File.Delete($"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{fileName}");
            }

            _db.Categories.Remove(categoryToDelete);
            bool changesPersitsToDb = await PersistChangesToDatabase();

            if (!changesPersitsToDb)
            {
                return StatusCode(500, "Error");
            }
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
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
