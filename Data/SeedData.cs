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
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserEmail, string testUserPw)
        {   
            using (var context = new TimeTrackDataContext(
                serviceProvider.GetRequiredService<DbContextOptions<TimeTrackDataContext>>()))
            {     
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, testUserEmail);
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

            var testSprint = new Sprint
            {
                SprintNumber = "Sprint1",
                StartDate = DateTime.Now.Date.AddDays(-1),
                StopDate = DateTime.Now.Date.AddDays(5),
            };

            var testTask = new Issue
            {
                TaskNumber = "VIR-1",
                TaskDescription = "Some Mac task to test",
                Sprint = testSprint,
                Platform = Platform.OSX
            };

            context.TimeTrack.AddRange(            
                new TimeTrack
                {
                    SpentHours = 1.2f,
                    TrackingDate = DateTime.Now.Date,
                    OwnerID = adminID,
                    Issue = testTask
                }                
            );

            var testTask2 = new Issue
            {
                TaskNumber = "VIR-2",
                TaskDescription = "Some Windows task without sprint",
                Sprint = null,
                Platform = Platform.Windows
            };

            context.TimeTrack.AddRange(            
                new TimeTrack
                {
                    SpentHours = 1f,
                    TrackingDate = DateTime.Now.Date.AddDays(-1),
                    OwnerID = adminID,
                    Issue = testTask2
                }                
            );

            context.Issue.AddRange(testTask2);

            var testTask3 = new Issue
            {
                TaskNumber = "VIR-3",
                TaskDescription = "Some task without sprint and platform",
                Sprint = null                
            };

            context.TimeTrack.AddRange(            
                new TimeTrack
                {
                    SpentHours = 0.5f,
                    TrackingDate = DateTime.Now.Date,
                    OwnerID = adminID,
                    Issue = testTask3
                }                
            );

            context.Issue.AddRange(testTask3);

            context.BusinessConstant.AddRange(
                new BusinessConstant
                {
                    ConstantName = Constants.ProductionMultiplier,
                    ConstantValue = 2.0f,
                    Description = "Production estimation time multiplier value"
                }
            );

            context.SaveChanges();
        }
    }
}