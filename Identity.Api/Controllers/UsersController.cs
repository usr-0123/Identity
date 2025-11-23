using System.Security.Claims;
using Identity.Api.DTO;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(UserManager<AppUser> userManager) : ControllerBase
{
    // ----------------------------------------------------------
    // GET MY PROFILE
    // ----------------------------------------------------------
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(OpenIddictConstants.Claims.Subject);

        var badResponse = NotFound(
            new ResponseDto<object> {
                Success = false,
                Message = "User not found."
            });
        
        if (userId == null)
        {
            return badResponse;
        }
        
        var user = await userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            return badResponse;
        }

        var userDto = new UserDto();

        return Ok(new ResponseDto<UserDto>
        {
            Success = true,
            Message = "User profile retrieved successfully.",
            Data = userDto
        });
    }
}