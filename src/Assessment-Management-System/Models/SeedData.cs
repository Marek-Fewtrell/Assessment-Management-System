using System;
using System.Linq;
using Assessment_Management_System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assessment_Management_System.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {

                RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                createRoles(roleManager);
                UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                //createAdmin(userManager);

                createAdmin(context, userManager);
                createSampleAssessmentData(context);
                
            }
        }

        public static void createSampleAssessmentData(ApplicationDbContext context)
        {
            //Look for any movies.
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

        public static async void createAdmin(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            /*
            //var adminUser = await userManager.FindByNameAsync("admin@email.com");

            //if (adminUser == null)
            //{
            var adminUser = new ApplicationUser { UserName = "admin@email.com", Email = "admin@email.com" };
            
            var result = await userManager.CreateAsync(adminUser, "Adm1n!");
            //}
            if (result.Succeeded)
            {
                if (!await userManager.IsInRoleAsync(adminUser, "admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "admin");
                }
            }*/

            var adminUser = new ApplicationUser { UserName = "admin@email.com", Email = "admin@email.com", };
            
            context.Users.Add(adminUser);
            //var roleConnection = new IdentityUserRole<IdentityRole<ApplicationUser>>();
            //context.UserRoles.Add()
            context.SaveChanges();

            if (!await userManager.IsInRoleAsync(adminUser, "admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "admin");
            }


        }

    }
}
