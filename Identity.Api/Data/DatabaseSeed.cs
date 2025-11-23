using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

namespace Identity.Api.Data;

public static class DatabaseSeed
{
    public static async Task SeedRolesAndClients(IServiceProvider provider)
    {
        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        var appManager = provider.GetRequiredService<IOpenIddictApplicationManager>();
        
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        
        if (await appManager.FindByClientIdAsync("default-client") == null)
        {
            await appManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "default-client",
                ClientSecret = "super-secret-password",
                DisplayName = "Default Client Application",
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Email
                }
            });
        }
    }
}