using Shared.Models.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class PostDto
{
    [Key]
    public int PostId { get; set; }

    [Required]
    [MaxLength(128)]

    [NoPeriods(ErrorMessage = "The post title field contains one or more periods (.).")]
    [NoThreeOrMoreSpacesInARow(ErrorMessage = "The psot title field contains three or more space in a row.")]
    public string Title { get; set; }

    [Required]
    [MaxLength(256)]
    public string ThumbnailImagePath { get; set; }

    [Required]
    [MaxLength(512)]
    public string Excerpt { get; set; }

    public string Content { get; set; }

    [Required]
    public bool IsPublished { get; set; }

    [Required]
    [MaxLength(128)]
    public string Author { get; set; }

    [Required]
    public int CategoryId { get; set; }
}
