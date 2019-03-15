using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TimeTracking.Models;

namespace TimeTracking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetConfig();

            var port = GetApplicationPort(configuration);

            var host = CreateWebHostBuilder(args)
                            .UseUrls($"http://0.0.0.0:{port}")
                            .Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<TimeTrackDataContext>();
                context.Database.Migrate();

                // requires using Microsoft.Extensions.Configuration;
                // var config = host.Services.GetRequiredService<IConfiguration>();
                // Set password with the Secret Manager tool.
                // dotnet user-secrets set SeedUserPW <pw>

                var testUserEmail = configuration["SeedUserEmail"];
                var testUserPw = configuration["SeedUserPW"];
                                
                SeedData.Initialize(services, testUserEmail, testUserPw).Wait();
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static IConfiguration GetConfig()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");

            return builder.Build();
        }

        private static int GetApplicationPort(IConfiguration configuration)
        {
            var portStr = configuration["Port"];
            if (string.IsNullOrEmpty(portStr))
                throw new ApplicationException("Port is not specified in configuration");
            var port = int.Parse(portStr);
            if (port <= 0)
                throw new ApplicationException("Port should be a positive number");

            return port;
        }
    }
}
