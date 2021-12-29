using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Call the base version of this method as well, else we get an error later on
        base.OnModelCreating(modelBuilder);

        Category[] categoriesToSeed = new Category[3];

        for (int i = 0; i < categoriesToSeed.Length; i++)
        {
            categoriesToSeed[i - 1] = new Category
            {
                CategoryId = i,
                ThumbnailImagePath = "uploads/placeholder.jpg",
                Name = $"Category {i}",
                Description = $"A description of category {1}"
            };
        }

        modelBuilder.Entity<Category>().HasData(categoriesToSeed);
    }
}
