using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models;

public class RegisterViewModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password), MinLength(8)]
    public string Password { get; set; } = "";

    [Required, DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "De wachtwoorden komen niet overeen.")]
    public string ConfirmPassword { get; set; } = "";
}
