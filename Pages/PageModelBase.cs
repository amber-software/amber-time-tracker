using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;
using TimeTracking.Services.Sprints;

namespace TimeTracking.Pages
{    
    public class PageModelBase : PageModel
    {
        protected readonly TimeTracking.Models.TimeTrackDataContext context;
        protected readonly UserManager<IdentityUser> userManager;
        protected IAuthorizationService authorizationService { get; }
        protected ISprintsService sprintsService { get; }

        public PageModelBase(TimeTracking.Models.TimeTrackDataContext context,
                            IAuthorizationService authorizationService,
                            UserManager<IdentityUser> userManager,
                            ISprintsService sprintsService)
                                  : base()
        {
            this.context = context;
            this.userManager = userManager;
            this.authorizationService = authorizationService;
            this.sprintsService = sprintsService;
        }        
    }
}