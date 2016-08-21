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
                UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                createRoles(roleManager);
                seedAssessments(context);
                seedUsers(context, userManager, "student@email.com", "Student1!", "student");
                seedUsers(context, userManager, "teacher@email.com", "Teacher1!", "tutor");
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

        public static async void seedUsers(ApplicationDbContext context, UserManager<ApplicationUser> userManager, string email, string password, string role)
        {
            if (context.Users.Any(e => e.Email == email))
            {
                return;
            }

            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        public static void seedAssessments(ApplicationDbContext context)
        {
            //Look for any Assessment
            if (context.Assessment.Any())
            {
                return; //DB has been seeded.
            }

            context.Assessment.AddRange(
                new Assessment
                {
                    Title = "Assessment 1",
                    Description = "Assessment 1 Description",
                    DueDate = DateTime.Parse("2016-07-20T21:00:00.00")
                },
                new Assessment
                {
                    Title = "Assessment 2",
                    Description = "Assessment 2 Description",
                    DueDate = DateTime.Parse("2016-07-20T21:00:00.00")
                }
            );
            context.SaveChanges();
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
