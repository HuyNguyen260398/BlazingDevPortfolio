using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Call the base version of this method as well, else we get an error later on
        base.OnModelCreating(modelBuilder);

        #region Category seed

        Category[] categoriesToSeed = new Category[3];

        for (int i = 1; i < 4; i++)
        {
            categoriesToSeed[i - 1] = new Category
            {
                CategoryId = i,
                ThumbnailImagePath = "uploads/placeholder.jpg",
                Name = $"Category {i}",
                Description = $"A description of category {i}"
            };
        }

        modelBuilder.Entity<Category>().HasData(categoriesToSeed);

        #endregion

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Posts)
                  .HasForeignKey("CategoryId");
        });

        #region Post seed

        Post[] postsToSeed = new Post[6];

        for (int i = 1; i < 7; i++)
        {
            int categoryId = 0;

            switch (i)
            {
                case 1:
                    categoryId = 1;
                    break;
                case 2:
                    categoryId = 1;
                    break;
                case 3:
                    categoryId = 2;
                    break;
                case 4:
                    categoryId = 2;
                    break;
                case 5:
                    categoryId = 3;
                    break;
                case 6:
                    categoryId = 3;
                    break;
                default:
                    break;
            }

            postsToSeed[i - 1] = new Post
            {
                PostId = i,
                ThumbnailImagePath = "/uploads/placeholder.jpg",
                Title = $"Post {i}",
                Excerpt = $"This is an excerpt for post {i}",
                Content = String.Empty,
                PublishDate = DateTime.UtcNow.ToString(),
                IsPublished = true,
                Author = "admin",
                CategoryId = categoryId,
            };
        }

        modelBuilder.Entity<Post>().HasData(postsToSeed);

        #endregion
    }
}
