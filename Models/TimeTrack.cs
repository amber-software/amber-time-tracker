using System;
using System.Collections.Generic;

namespace TimeTracking.Models
{
    public class TimeTrack
    {
        public int ID { get; set; }
        public int SpentHours { get; set; }
        public DateTime TrackingDate { get; set; }    
        public Issue Issue { get; set; }
        public string OwnerID { get; set; }
    }
}