using System;
using Microsoft.AspNetCore.Mvc;

namespace DeltaWorkMonitoring.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult Error() => View();
    }
}
