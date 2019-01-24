using ASR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASR.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { Constants.StaffRole, Constants.StudentRole };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = role });
                }
            }
            var userManager = serviceProvider.GetService<UserManager<AccountUser>>();
        }

        //private static async Task EnsureUserHasRole(
        //UserManager<AccountUser> userManager, string userName, string role)
        //{
        //    var user = await userManager.FindByNameAsync(userName);
        //    if (user != null && !await userManager.IsInRoleAsync(user, role))
        //    {
        //        await userManager.AddToRoleAsync(user, role);
        //    }
        //}


    }
}
