using System;
using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Models
{
    public class TimeTrack
    {
        public int ID { get; set; }

        [Required]
        public float SpentHours { get; set; }

        [Required]
        [DataType(DataType.Date)]

        public DateTime TrackingDate { get; set; }    

        public int IssueID { get; set; }

        public Issue Issue { get; set; }
        
        public string OwnerID { get; set; }

        public string Description { get; set; }

        public Platform Platform { get; set; }
    }
}