using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DeltaWorkMonitoring.Infrastructure;

namespace DeltaWorkMonitoring.Models
{
    public class TaskRepository : ITaskRepository
    {
        private ApplicationDbContext context;

        public TaskRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        //public IEnumerable<WorkTask> GetTasks(Guid userId)
        //{
        //    var now = DateTime.Now;
        //    var ret = context.Tasks
        //        .Include(e => e.Project)
        //        .Include(e => e.TaskHistoryItems)
        //        .Where(t => t.Created.HasValue && t.UserId == userId &&
        //        t.Created.Value.Month.Equals(now.Month) &&
        //        t.Created.Value.Year.Equals(now.Year));

        //    return ret;
        //}

        public IEnumerable<WorkTask> GetTasks(Guid userId, TaskPeriod period)
        {
            DateTime to = DateTime.Now;
            DateTime from;
            switch (period)
            {
                case TaskPeriod.Day:
                    from = to.GetDayStartDate();
                    break;
                case TaskPeriod.Week:
                    from = to.GetWeekStartDate();
                    break;
                case TaskPeriod.Month:
                    from = to.GetMonthStartDate();
                    break;
                case TaskPeriod.Quarter:
                    from = to.GetQuarterStartDate();
                    break;
                case TaskPeriod.Year:
                    from = to.GetYearStartDate();
                    break;
                case TaskPeriod.All:
                    from = to.AddYears(-100);
                    break;
                default:
                    throw new ArgumentException(nameof(period));
            }

            var ret = context.GetTasks()
                .Include(e => e.Project)
                .Include(e => e.TaskHistoryItems)
                .Where(t => t.UserId == userId && 
                    t.Created.HasValue && t.Created >= from && t.Created <= to);

            return ret;
        }

        public IEnumerable<Project> Projects
        {
            get
            {
                return context.Projects.OrderBy(p => p.Name);
            }
        }

        public void SaveTask(WorkTask task, Guid userId)
        {
            if(task.Id == new Guid())
            {
                AddTask(task, userId);
            }
            else
            {
                UpdateTask(task);
            }
        }

        private void AddTask(WorkTask task, Guid userId)
        {
            var project = context.Projects.FirstOrDefault(p => p.Name.Equals(task.ProjectName));
            task.Project = project;
            task.UserId = userId;

            context.Tasks.Add(task);
            context.SaveChanges();
            if (task.Started.HasValue && task.Finished.HasValue)
            {
                context.TaskHistory.Add(new TaskHistoryItem { Task = task, Status = TaskStatus.InProgress, Created = task.Started });
                context.TaskHistory.Add(new TaskHistoryItem { Task = task, Status = TaskStatus.Closed, Created = task.Finished });
            }
            if (task.Status.Equals(TaskStatus.InProgress))
            {
                context.TaskHistory.Add(new TaskHistoryItem { Task = task, Status = TaskStatus.InProgress, Created = DateTime.Now });
            }
            context.SaveChanges();
        }

        private void UpdateTask(WorkTask task)
        {
            var dbEntry = context.Tasks
                .Include(e => e.TaskHistoryItems)
                .FirstOrDefault(t => t.Id == task.Id);
            if (dbEntry == null) return;

            var isChanged = false;

            if (!string.IsNullOrWhiteSpace(task.Name) && (dbEntry.Name == null || !dbEntry.Name.Equals(task.Name)))
            {
                dbEntry.Name = task.Name;
                isChanged = true;
            }
            if (!string.IsNullOrWhiteSpace(task.Description) && (dbEntry.Description == null || !dbEntry.Description.Equals(task.Description)))
            {
                dbEntry.Description = task.Description;
                isChanged = true;
            }
            if (!string.IsNullOrWhiteSpace(task.Comment) && (dbEntry.Comment == null || !dbEntry.Comment.Equals(task.Comment)))
            {
                dbEntry.Comment = task.Comment;
                isChanged = true;
            }
            if (!dbEntry.Priority.Equals(task.Estimate))
            {
                dbEntry.Priority = task.Priority;
                isChanged = true;
            }
            if (!dbEntry.Estimate.Equals(task.Estimate))
            {
                dbEntry.Estimate = task.Estimate;
                isChanged = true;
            }
            if (!dbEntry.Status.Equals(task.Status))
            {
                if (dbEntry.Equals(TaskStatus.Stopped) && task.Status.Equals(TaskStatus.Closed))
                {
                    var taskHistoryItem = task.LastHistoryItem;
                    taskHistoryItem.Status = TaskStatus.Closed;
                }
                else
                {
                   context.TaskHistory.Add(new TaskHistoryItem { Task = task, Status = task.Status, Created = DateTime.Now });
                }
                isChanged = true;
            }
            
            if (isChanged)
            {
                task.Modified = DateTime.Now;
                context.SaveChanges();
            }
        }

        public WorkTask DeleteTask(Guid taskId)
        {
            var dbEntry = context.Tasks.FirstOrDefault(t => t.Id == taskId);
            if(dbEntry != null)
            {
                context.Tasks.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
    }
}
