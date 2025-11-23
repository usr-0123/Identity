using System.Security.Claims;
using Identity.Api.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<IdentityUser> userManager,
    IHttpContextAccessor httpContextAccessor)
    : ControllerBase
{
    // ----------------------------------------------------------
    // REGISTER USER
    // ----------------------------------------------------------
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new IdentityUser
        {
            UserName = dto.Email,
            Email = dto.Email,
        };
        
        var result = await userManager.CreateAsync(user, dto.Password);
        
        if (!result.Succeeded)
        {
            return BadRequest(new ResponseDto<object>
            {
                Success = false,
                Message = "User registration failed.",
            });
        }
        
        return Ok(new ResponseDto<object>
        {
            Success = true,
            Message = "User registered successfully."
        });
    }
    
    // ----------------------------------------------------------
    // LOGIN USER
    // ----------------------------------------------------------
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            return Unauthorized(new ResponseDto<object>
            {
                Success = false,
                Message = "Invalid auth credentials."
            });
        }

        if (!await userManager.CheckPasswordAsync(user, dto.Password))
        {
            return Unauthorized(new ResponseDto<object>
            {
                Success = false,
                Message = "Invalid auth credentials. Please try again."
            });
        }
        
        // Build token request
        var claims = new List<Claim>
        {
            new Claim(OpenIddictConstants.Claims.Subject, user.Id),
            new Claim(OpenIddictConstants.Claims.Email, user.Email ?? string.Empty),
            new Claim("fullName", user.UserName ?? string.Empty)
        };
        
        var identity = new ClaimsIdentity(claims, TokenValidationParameters.DefaultAuthenticationType);
        
        var principal = new ClaimsPrincipal(identity);
        principal.SetScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Scopes.OfflineAccess);
        
        // Issue token
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    
    // ----------------------------------------------------------
    // REFRESH TOKEN
    // ----------------------------------------------------------
    [HttpPost("refresh")]
    public IActionResult Refresh()
    {
        var principal = httpContextAccessor.HttpContext?.User;
        if (principal == null)
        {
            return Unauthorized(new ResponseDto<object>
            {
                Success = false,
                Message = "Invalid refresh token."
            });
        }
        principal.SetScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Email, OpenIddictConstants.Scopes.OfflineAccess);
        
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}