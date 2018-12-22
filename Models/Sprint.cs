using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Models
{
    public class Sprint
    {
        public int ID { get; set; }        

        [Required(AllowEmptyStrings=false)]
        public string SprintNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }    
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime StopDate { get; set; }

        public ICollection<Issue> Issues { get; set; }                        
    }
}