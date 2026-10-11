using System.ComponentModel.DataAnnotations;

namespace MediaWebApp.Models;

public class MediaEntry
{
    public int Id { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    [Required, StringLength(40)] public string MediaType { get; set; } = string.Empty;
    [StringLength(80)] public string? Genre { get; set; }
    [Range(1, 10)] public int Rating { get; set; }
    [Url, StringLength(500)] public string? CoverImageUrl { get; set; }
    [Required] public string OwnerId { get; set; } = string.Empty;
    public int ListId { get; set; }
    public MediaList List { get; set; } = null!;
}
