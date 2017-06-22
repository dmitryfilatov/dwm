using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeltaWorkMonitoring.Models
{
    public enum Cities
    {
        None = 0,
        Moscow,
        Rostov,
        Paris,
        Chicago
    }

    public enum QualificationLevels
    {
        None = 0,
        Basic,
        Advanced
    }

    public class AppUser : IdentityUser
    {
        public Cities City { get; set; }
        public QualificationLevels Qualifications { get; set; }
    }
}
