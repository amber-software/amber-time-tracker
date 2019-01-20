using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Models
{
    public class Issue
    {
        public int ID { get; set; }
        
        [Required(AllowEmptyStrings=false)]
        public string TaskNumber { get; set; }
        
        [Required(AllowEmptyStrings=false)]
        public string TaskDescription { get; set; }

        public int? SprintID { get; set; }

        public Sprint Sprint { get; set; }
        
        public int Estimate { get; set; }

        public int Remaining { get; set; }

        public Status Status { get; set; }

        public TaskPriority Priority { get; set; }

        public ICollection<TimeTrack> TimeTracks { get; set; }

        public Platform Platform { get; set; }
    }
}
