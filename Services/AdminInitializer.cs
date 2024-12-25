using AutoFull.Models;
using Microsoft.AspNetCore.Identity;

namespace AutoFull.Services;

public class AdminInitializer
{
    public static async Task SeedRolesAndAdmin(RoleManager<IdentityRole<int>> roleManager, UserManager<MyUser> _userManager)
    {
        string adminEmail = "admin@gmail.com";
        string adminPassword = "1qwe@QWE";
        string adminUserName = "admin";

        var roles = new[] { "admin", "user"};

        foreach (var role in roles)
        {
            if(await roleManager.FindByNameAsync(role) is null)
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        if (await _userManager.FindByEmailAsync(adminEmail) == null)
        {
            MyUser adminUser = new MyUser { Email = adminEmail, UserName = adminUserName };
            IdentityResult result = await _userManager.CreateAsync(adminUser, adminPassword);
            if(result.Succeeded)
                await _userManager.AddToRoleAsync(adminUser, "admin");
        }
    }
}