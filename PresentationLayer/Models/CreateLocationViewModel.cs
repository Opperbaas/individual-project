using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models;

public sealed class CreateLocationViewModel
{
    [Required]
    [StringLength(200)]
    [Display(Name = "Naam")]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(100)]
    [Display(Name = "Type")]
    public string LocationType { get; set; } = "";

    [Range(1, int.MaxValue)]
    [Display(Name = "Capaciteit")]
    public int Capacity { get; set; } = 1;

    [StringLength(1000)]
    [Display(Name = "Beschrijving")]
    public string Description { get; set; } = "";
}
