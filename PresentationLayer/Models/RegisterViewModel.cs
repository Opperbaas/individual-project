using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models;

public class RegisterViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = "";

    [Required, DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "De wachtwoorden komen niet overeen.")]
    public string ConfirmPassword { get; set; } = "";
}
