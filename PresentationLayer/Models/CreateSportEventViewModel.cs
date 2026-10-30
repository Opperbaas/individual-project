using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models;

public sealed class CreateSportEventViewModel
{
    [Required]
    [StringLength(200)]
    [Display(Name = "Naam")]
    public string EventName { get; set; } = "";

    [StringLength(1000)]
    [Display(Name = "Beschrijving")]
    public string Description { get; set; } = "";

    [Required]
    [Display(Name = "Begindatum")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Einddatum")]
    public DateTime EndDate { get; set; } = DateTime.Today;
}
