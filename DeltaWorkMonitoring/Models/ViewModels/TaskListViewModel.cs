using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeltaWorkMonitoring.Models.ViewModels
{
    public class TaskListViewModel
    {
        public IEnumerable<WorkTask> Tasks { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public TaskStatus CurrentStatus { get; set; }
    }
}
