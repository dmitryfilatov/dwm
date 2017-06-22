using System;
using System.Collections.Generic;
using System.Linq;

namespace DeltaWorkMonitoring.Models
{
    public interface ITaskRepository
    {
        IEnumerable<WorkTask> GetTasks(Guid userId, TaskPeriod period);
        IEnumerable<Project> Projects { get; }
        void SaveTask(WorkTask task, Guid userId);
        WorkTask DeleteTask(Guid taskId);
    }
}
