using System;
using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Models
{
    public class TimeTrack
    {
        public int ID { get; set; }
        public float SpentHours { get; set; }

        [DataType(DataType.Date)]
        public DateTime TrackingDate { get; set; }    
        public int IssueID { get; set; }
        public Issue Issue { get; set; }
        public string OwnerID { get; set; }
    }
}