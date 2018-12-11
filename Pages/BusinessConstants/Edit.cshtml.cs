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
using TimeTracking.Authorization;
using TimeTracking.Models;

namespace TimeTracking.Pages.BusinessConstants
{
    public class EditModel : PageModelBase
    {        
        public EditModel(TimeTracking.Models.TimeTrackDataContext context,
                          IAuthorizationService authorizationService,
                          UserManager<IdentityUser> userManager)
                                  : base(context, authorizationService, userManager)
        {            
        }

        [BindProperty]
        public BusinessConstant BusinessConstant { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BusinessConstant = await context.BusinessConstant.FirstOrDefaultAsync(m => m.ID == id);

            if (BusinessConstant == null)
            {
                return NotFound();
            }

            var isAuthorized = await authorizationService.AuthorizeAsync(
                                                      User, BusinessConstant,
                                                      BusinessConstantsOperations.EditBusinessConstants);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var isAuthorized = await authorizationService.AuthorizeAsync(
                                                      User, BusinessConstant,
                                                      BusinessConstantsOperations.EditBusinessConstants);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            context.Attach(BusinessConstant).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusinessConstantExists(BusinessConstant.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BusinessConstantExists(int id)
        {
            return context.BusinessConstant.Any(e => e.ID == id);
        }
    }
}
