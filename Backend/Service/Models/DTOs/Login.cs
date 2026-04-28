using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs;

public class Login
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Username must be between 1 and 50 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}
