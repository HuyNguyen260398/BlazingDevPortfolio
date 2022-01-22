using Microsoft.AspNetCore.Identity;

namespace Server.Data;

public static class SeedAdminRolesAndUsers
{
    internal const string AdminRoleName = "Admin";
    internal const string AdminUserName = "admin@gmail.com";

    internal static async Task Seed(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        await SeedAdminRole(roleManager);
        await SeedAdminUser(userManager);
    }

    private static async Task SeedAdminRole(RoleManager<IdentityRole> roleManager)
    {
        bool adminRoleExists = await roleManager.RoleExistsAsync(AdminRoleName);

        if (!adminRoleExists)
        {
            var role = new IdentityRole
            {
                Name = AdminRoleName,
            };
            await roleManager.CreateAsync(role);
        }
    }

    private static async Task SeedAdminUser(UserManager<IdentityUser> userManager)
    {
        bool adminUserExists = await userManager.FindByEmailAsync(AdminUserName) != null;

        if (!adminUserExists)
        {
            var user = new IdentityUser
            {
                Email = AdminUserName,
                UserName = AdminUserName,
            };
            
            // Make sure your git repo is private if you do this
            IdentityResult identityResult = await userManager.CreateAsync(user, "P@$$w0rd");

            if (identityResult.Succeeded)
                await userManager.AddToRoleAsync(user, AdminRoleName);
        }
    }
}
