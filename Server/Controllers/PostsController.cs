using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;

namespace Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PostsController(AppDbContext db, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _webHostEnvironment = webHostEnvironment;
    }

    #region  CRUD Operations

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var Posts = await _db.Posts.ToListAsync();
        return Ok(Posts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        Post post = await GetPostByPostId(id);
        return Ok(post);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Post postToCreate)
    {
        try
        {
            if (postToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (postToCreate.IsPublished)
            {
                postToCreate.PublishDate = DateTime.Now.ToShortDateString();
            }

            await _db.Posts.AddAsync(postToCreate);
            bool changesPersitsToDb = await PersistChangesToDatabase();

            if (!changesPersitsToDb)
            {
                return StatusCode(500, "Error");
            }
            return Created("Create", postToCreate);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Post updatedPost)
    {
        try
        {
            if (id < 1 || updatedPost == null || id != updatedPost.PostId)
            {
                return BadRequest(ModelState);
            }

            var oldPost = await _db.Posts.FindAsync(id);

            if (oldPost == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (oldPost.IsPublished && updatedPost.IsPublished)
            {
                updatedPost.PublishDate = DateTime.Now.ToShortDateString();
            }

            // Detach oldPost from EF, else it cannot be updated
            _db.Entry(oldPost).State = EntityState.Detached;
            _db.Posts.Update(updatedPost);
            bool changesPersitsToDb = await PersistChangesToDatabase();

            if (!changesPersitsToDb)
            {
                return StatusCode(500, "Error");
            }
            return Created("create", updatedPost);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id < 1)
            {
                return BadRequest(ModelState);
            }

            bool isExsisted = await _db.Posts.AnyAsync(c => c.PostId == id);
            if (!isExsisted)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Post PostToDelete = await GetPostByPostId(id);
            if (PostToDelete.ThumbnailImagePath != "uploads/placeholder.jpg")
            {
                string fileName = PostToDelete.ThumbnailImagePath.Split('/').Last();
                System.IO.File.Delete($"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{fileName}");
            }

            _db.Posts.Remove(PostToDelete);
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
    private async Task<Post> GetPostByPostId(int PostId)
    {
        Post PostToGet = null;
        PostToGet = await _db.Posts.Include(p => p.Category).FirstAsync(c => c.PostId == PostId);
        return PostToGet;
    }

    #endregion
}
