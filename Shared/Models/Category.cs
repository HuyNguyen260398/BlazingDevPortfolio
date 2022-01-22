using Shared.Models.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(256)]
    public string ThumbnailImagePath { get; set; }

    [Required]
    [MaxLength(128)]
    [NoPeriods(ErrorMessage = "The category name field contains one or more periods (.).")]
    [NoThreeOrMoreSpacesInARow(ErrorMessage = "The category name field contains three or more space in a row.")]
    public string Name { get; set; }

    [Required]
    [MaxLength(1024)]
    public string Description { get; set; }

    public List<Post> Posts { get; set; }
}
