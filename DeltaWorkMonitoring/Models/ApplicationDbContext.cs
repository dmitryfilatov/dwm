using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeltaWorkMonitoring.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<WorkTask> Tasks { get; set; }
        public DbSet<TaskHistoryItem> TaskHistory { get; set; }

        public virtual DbSet<WorkTask> GetTasks()
        {
            return Tasks;
        }
    }
}
