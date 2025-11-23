using System.ComponentModel.DataAnnotations;

namespace Identity.Api.DTO;

public class LoginDto
{
    [Required(ErrorMessage = "Please provide your email address!", AllowEmptyStrings = false)]
    public required string Email { get; set; }
    [Required(ErrorMessage = "Please provide your password!", AllowEmptyStrings = false)]
    public required string Password { get; set; }
}