using System.ComponentModel.DataAnnotations;

namespace MediaWebApp.Models;

public class MediaEntryInput
{
    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    [Required, StringLength(40), Display(Name = "Media type")] public string MediaType { get; set; } = string.Empty;
    [StringLength(80)] public string? Genre { get; set; }
    [Range(1, 10)] public int Rating { get; set; } = 1;
    [Url, StringLength(500), Display(Name = "Cover image URL")] public string? CoverImageUrl { get; set; }
}
