using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class Post
{
    [Key]
    public int PostId { get; set; }

    [Required]
    [MaxLength(128)]
    public string Title { get; set; }

    [Required]
    [MaxLength(256)]
    public string ThumbnailImagePath { get; set; }

    [Required]
    [MaxLength(512)]
    public string Excerpt { get; set; }

    public string Content { get; set; }

    public string PublishDate { get; set; }

    [Required]
    public bool IsPublished { get; set; }

    [Required]
    [MaxLength(128)]
    public string Author { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public Category Category { get; set; }
}
