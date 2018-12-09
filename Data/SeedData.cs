using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TimeTracking.Authorization;

namespace TimeTracking.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {   
            using (var context = new TimeTrackDataContext(
                serviceProvider.GetRequiredService<DbContextOptions<TimeTrackDataContext>>()))
            {     
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@contoso.com");
                await EnsureRole(serviceProvider, adminID, Constants.AdministratorsRole);               
         
                SeedDB(context, adminID);
            }
        }   

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser { UserName = UserName };
                await userManager.CreateAsync(user, testUserPw);
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                              string uid, string role)
        {
           IdentityResult IR = null;
           var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

           if (roleManager == null)
           {
               throw new Exception("roleManager null");
           }

           if (!await roleManager.RoleExistsAsync(role))
           {               
               IR = await roleManager.CreateAsync(new IdentityRole(role));
           }

           var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

           var user = await userManager.FindByIdAsync(uid);

           IR = await userManager.AddToRoleAsync(user, role);           

           return IR;
        }

        public static void SeedDB(TimeTrackDataContext context, string adminID)
        {
            if (context.TimeTrack.Any())
            {
                return;   // DB has been seeded
            }

            context.TimeTrack.AddRange(            
                new TimeTrack
                {
                    SpentHours = 1.2f,
                    TrackingDate = new DateTime(2018, 11, 1),
                    OwnerID = adminID,
                    Issue = new Issue
                    {
                        TaskNumber = "VIR-1",
                        TaskDescription = "Some task to test"
                    }
                }                
            );

            context.Issue.AddRange(            
                new Issue
                {
                    TaskNumber = "VIR-2",
                    TaskDescription = "Another task to test"
                },
                new Issue
                {
                    TaskNumber = "VIR-3",
                    TaskDescription = "Third task to test"
                }  
            );

            context.SaveChanges();
        }
    }
}