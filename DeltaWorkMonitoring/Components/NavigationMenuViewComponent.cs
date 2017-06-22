using DeltaWorkMonitoring.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeltaWorkMonitoring.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private ITaskRepository repository;
        private UserManager<AppUser> userManager;

        public NavigationMenuViewComponent(ITaskRepository repo, UserManager<AppUser> usrMgr)
        {
            repository = repo;
            userManager = usrMgr;
        }

        public IViewComponentResult Invoke()
        {
            var userId = userManager.GetUserId(HttpContext.User);
            ViewBag.SelectedStatus = RouteData?.Values["status"];
            return View(repository.GetTasks(new Guid(userId), TaskPeriod.Quarter)
                .Select(x => x.Status)
                .Distinct()
                .OrderBy(x => x));
        }
    }
}
