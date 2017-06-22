using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace DeltaWorkMonitoring.Models
{
    public class TaskHistoryItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public WorkTask Task { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime? Created { get; set; }
    }
}
