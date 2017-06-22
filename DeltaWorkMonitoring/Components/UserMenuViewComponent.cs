using DeltaWorkMonitoring.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeltaWorkMonitoring.Components
{
    public class UserMenuViewComponent : ViewComponent
    {
        private UserManager<AppUser> userManager;

        public UserMenuViewComponent(UserManager<AppUser> usrMgr)
        {
            userManager = usrMgr;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            ViewBag.VisibleForAdmins = await userManager.IsInRoleAsync(user, "Admins");
            return View(user);
        }
    }
}
