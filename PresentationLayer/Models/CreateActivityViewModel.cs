using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models;

public sealed class CreateActivityViewModel
{
    public int EventId { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Naam")]
    public string ActivityName { get; set; } = "";

    [Required]
    [StringLength(100)]
    [Display(Name = "Type")]
    public string ActivityType { get; set; } = "";

    [StringLength(1000)]
    [Display(Name = "Beschrijving")]
    public string Description { get; set; } = "";

    [Range(1, int.MaxValue)]
    [Display(Name = "Maximum aantal deelnemers")]
    public int MaxParticipants { get; set; } = 1;
}
