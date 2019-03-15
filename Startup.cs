using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using TimeTracking.Authorization;
using TimeTracking.Services.Sprints;
using TimeTracking.Services.Issues;
using Microsoft.Extensions.Logging;

namespace TimeTracking
{
    public class Startup
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            services.AddDbContext<TimeTrackDataContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("TimeTrackDataContext")));

            services.AddIdentity<IdentityUser, IdentityRole>()                
                .AddEntityFrameworkStores<TimeTrackDataContext>()
                .AddDefaultTokenProviders();
            
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = true;                
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.ConfigureApplicationCookie(options =>
            {                
                options.LoginPath = "/Identity/Account/Login";                
            });

            services.AddMvc(config =>
                    {
                        // using Microsoft.AspNetCore.Mvc.Authorization;
                        // using Microsoft.AspNetCore.Authorization;
                        var policy = new AuthorizationPolicyBuilder()
                                         .RequireAuthenticatedUser()
                                         .Build();
                        config.Filters.Add(new AuthorizeFilter(policy));
                    })                    
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IAuthorizationHandler,
                                  BusinessConstantAdministratorsAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler,
                                  SprintsAdministratorsAuthorizationHandler>();
                                  
            services.AddScoped<IAuthorizationHandler,                                  
                                  TasksAdministratorsAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler,                           
                                  TimeTrackAdministratorsAuthorizationHandler>();

            services.AddTransient<ISprintsService, SprintsService>();
            services.AddTransient<IIssueService, IssueService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            applicationLifetime.ApplicationStarted.Register(ApplicationStarted);
            applicationLifetime.ApplicationStopped.Register(ApplicationStopped);

            loggerFactory.AddLog4Net();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            
            app.UseMvc();
        }

        private void ApplicationStopped()
        {
            log.Warn("----====Application Stopped====-----");
        }

        private void ApplicationStarted()
        {
            log.Warn("----====Application Started====-----");
        }
    }
}
