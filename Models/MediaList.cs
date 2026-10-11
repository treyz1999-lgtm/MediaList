using System.ComponentModel.DataAnnotations;

namespace MediaWebApp.Models;

public class MediaList
{
    public int Id { get; set; }
    [Required] public string OwnerId { get; set; } = string.Empty;
    [Required, StringLength(80)] public string Name { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public List<MediaEntry> Entries { get; set; } = [];
}
