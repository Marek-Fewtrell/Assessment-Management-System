using Assessment_Management_System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Assessment_Management_System.Models
{
    public class DbSetup
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {

                RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                createRoles(roleManager);

            }
        }

        public static async void createRoles(RoleManager<IdentityRole> roleManager)
        {
            // Create the necessary roles
            List<IdentityRole> roles = new List<IdentityRole>();
            roles.Add(new IdentityRole("admin"));
            roles.Add(new IdentityRole("teacher"));
            roles.Add(new IdentityRole("student"));

            // Add roles to the database
            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role.Name);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(role);
                }
            }

        }

        public static async void createClaim(RoleManager<IdentityRole> roleManager)
        {
            List<IdentityRole> roles = roleManager.Roles.ToList<IdentityRole>();
            var viewer = new Claim("Permission", "view");
            foreach (var role in roles)
            {
                switch (role.Name)
                {
                    case "student":
                        await roleManager.AddClaimAsync(role, new Claim("Permission", "view"));
                        break;
                    case "teacher":
                        break;
                    case "admin":
                        break;
                }
            }
        }
    }
}
