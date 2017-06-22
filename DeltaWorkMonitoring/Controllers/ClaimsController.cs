using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeltaWorkMonitoring.Controllers
{
    public class ClaimsController : Controller
    {
        [Authorize(Policy = "ROUsers")]
        public ViewResult OtherAction() => View("Index", User?.Claims);

        //[Authorize]
        [Authorize(Policy = "NoUser1")]
        public ViewResult Index() => View(User?.Claims);
    }
}
