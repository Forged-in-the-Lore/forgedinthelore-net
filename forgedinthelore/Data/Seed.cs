using System.Text.Json;
using forgedinthelore_net.DTOs;
using forgedinthelore_net.Entities;
using forgedinthelore_net.Errors;
using forgedinthelore_net.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace forgedinthelore_net.Data;

public static class Seed
{
    //Add new seeds by creating a seed data file Data/SeedData/*.json
    //and creating a Seed* method that takes the relevant services as parameters.
    public static async Task Run(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        await SeedUsers(userManager, roleManager);
    }

    private static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var roles = new List<AppRole>
        {
            new() {Name = "Admin"},
            new() {Name = "User"}
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        var userData = await File.ReadAllTextAsync("Data/SeedData/UserSeedData.json");
        var users = JsonSerializer.Deserialize<List<RegisterDto>>(userData);
        if (users == null) throw new SeedException("No users in seed file");
        foreach (var user in users)
        {
            var appUser = new AppUser
            {
                UserName = user.UserName.ToLower()
            };
            await userManager.CreateAsync(appUser, "Passw0rd!");
            await userManager.AddToRoleAsync(appUser, "admin");
        }
    }
}