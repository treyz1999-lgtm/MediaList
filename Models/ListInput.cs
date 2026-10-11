using System.ComponentModel.DataAnnotations;

namespace MediaWebApp.Models;

public class ListInput
{
    [Required, StringLength(80), Display(Name = "List name")]
    public string Name { get; set; } = string.Empty;
}
