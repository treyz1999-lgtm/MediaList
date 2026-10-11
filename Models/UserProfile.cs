using System.ComponentModel.DataAnnotations;

namespace MediaWebApp.Models;

public class UserProfile
{
    [Key] public string UserId { get; set; } = string.Empty;
    [Required, StringLength(60)] public string DisplayName { get; set; } = string.Empty;
}
