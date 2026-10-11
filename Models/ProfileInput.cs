using System.ComponentModel.DataAnnotations;

namespace MediaWebApp.Models;

public class ProfileInput
{
    [Required, StringLength(60, MinimumLength = 2), Display(Name = "Display name")]
    public string DisplayName { get; set; } = string.Empty;
}
