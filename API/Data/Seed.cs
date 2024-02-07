﻿using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
#nullable disable
public class Seed
{
    // public static async Task SeedUsers(DataContext dataContext)
    public static async Task SeedUsers(UserManager<AppUser> userManager) //
    {
        if (await userManager.Users.AnyAsync()) return; //

        var userSeedData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userSeedData, opt);
        foreach (var user in users)
        {
            user.UserName = user.UserName.ToLower();// แล้วแต่ชอบ
            await userManager.CreateAsync(user, "P@ssw0rd"); //
            // dataContext.Users.Add(user);
        }
        // await dataContext.SaveChangesAsync();
    }


}