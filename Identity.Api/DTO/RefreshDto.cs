using System.ComponentModel.DataAnnotations;

namespace Identity.Api.DTO;

public class RefreshDto
{
    public required string RefreshToken { get; set; }
    public required string ClientId { get; set; }
}