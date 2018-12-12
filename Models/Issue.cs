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

        public int SprintID { get; set; }
        public Sprint Sprint { get; set; }
    }
}