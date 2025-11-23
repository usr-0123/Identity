using System.ComponentModel.DataAnnotations;

namespace Identity.Api.DTO;

public class RegisterDto
{
    [Required(ErrorMessage = "Email address for your account is required", AllowEmptyStrings = false)]
    public required string Email { get; set; }
    [Required(ErrorMessage = "Password for your account is required", AllowEmptyStrings = false)]
    public required string Password { get; set; }
}