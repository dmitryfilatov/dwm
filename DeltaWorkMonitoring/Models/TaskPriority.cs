using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeltaWorkMonitoring.Models
{
    public enum TaskPriority
    {
        None = 0,
        Urgent,
        High,
        Important,
        Medium,
        Moderate,
        Low,
        DontFix
    }
}
