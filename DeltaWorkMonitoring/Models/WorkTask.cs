using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DeltaWorkMonitoring.Models
{

    public class TaskDateAttribute : ValidationAttribute
    {
        public TaskDateAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            if(value == null) return  true;
            var dt = (DateTime)value;
            if (dt < DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }

    public class WorkTask
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required (ErrorMessage = "Please enter a task name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public Guid UserId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a positive estimate")]
        public int Estimate { get; set; } = 1;
        virtual public Project Project { get; set; }
        //virtual public User User { get; set; }
        virtual public WorkTask Parent { get; set; }
        public DateTime? Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; }
        virtual public ICollection<TaskHistoryItem> TaskHistoryItems { get; set; }
        virtual public ICollection<WorkTask> Tasks { get; set; }

        [NotMapped]
        public TaskHistoryItem LastHistoryItem
        {
            get
            {
                return TaskHistoryItems?.OrderByDescending(x => x.Created).FirstOrDefault();
            }
        }

        private TaskStatus _status;
        [NotMapped]
        public TaskStatus Status
        {
            get
            {
                if (_status == TaskStatus.None)
                {
                    var taskHistoryItem = LastHistoryItem;
                    _status = taskHistoryItem == null ? TaskStatus.None : taskHistoryItem.Status;
                }
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        [NotMapped]
        public TimeSpan Elapsed
        {
            get
            {
                TimeSpan? result = new TimeSpan();
                var taskHistoryItems = TaskHistoryItems?.OrderBy(x => x.Created);
                if (taskHistoryItems == null) return result.GetValueOrDefault();
                DateTime? currInProgress = null;
                foreach (var item in taskHistoryItems)
                {
                    if (item.Status.Equals(TaskStatus.InProgress))
                    {
                        currInProgress = item.Created;
                    }
                    else if (currInProgress != null)
                    {
                        result += item.Created - currInProgress;
                        currInProgress = null;
                    }
                }
                if (currInProgress != null)
                {
                    result += DateTime.Now - currInProgress;
                }

                return result.GetValueOrDefault();
            }
        }

        [NotMapped]
        public double ElapsedHours
        {
            get
            {
                return Math.Ceiling(Elapsed.TotalHours);
            }
        }

        [NotMapped]
        public double? Velocity
        {
            get
            {
                return Elapsed.TotalHours == 0 ? (double?)null : Math.Round(Estimate / Elapsed.TotalHours, 1);
            }
        }

        private DateTime? _started;
        [NotMapped]
        [TaskDate(ErrorMessage = "Please specify a date earlier than the now one")]
        public DateTime? Started
        {
            get
            {
                if (_started == null)
                {
                    _started = TaskHistoryItems?.Where(e => e.Status.Equals(TaskStatus.InProgress)).Min(x => x.Created);
                }
                return _started;
            }
            set
            {
                _started = value;
            }
        }

        private DateTime? _finished;
        [NotMapped]
        [TaskDate(ErrorMessage = "Please specify a date earlier than the now one")]
        public DateTime? Finished
        {
            get
            {
                if (!Status.Equals(TaskStatus.Closed))
                    return null;

                if (_finished == null)
                {
                    _finished = TaskHistoryItems?.Where(e => e.Status.Equals(TaskStatus.Closed)).Max(x => x.Created);
                }
                return _finished;
            }
            set
            {
                _finished = value;
            }
        }

        [NotMapped]
        [Display(Name = "Task")]
        public string FullName
        {
            get
            {
                //return Utils.GetTaskFullName(Name, Description).GetTruncateWithSuffix(48, "..");
                return null;
            }
        }

        [NotMapped]
        public string Hours
        {
            get
            {
                return string.Format("{0} ({1})", ElapsedHours, Elapsed.ToString("hh\\:mm"));
            }
        }

        private string _projectName;
        [NotMapped]
        [Display(Name = "Project")]
        public string ProjectName
        {
            get
            {
                if (_projectName == null)
                {
                    _projectName = Project?.Name;
                }
                return _projectName;
            }
            set
            {
                _projectName = value;
            }
        }
    }
}
