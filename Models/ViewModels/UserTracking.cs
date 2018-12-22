using System;
using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Models
{
    public class UserTracking
    {
        public string UserID { get; set; }

        public string UserName { get; set; }
        
        public float TotalSpentHours { get; set; }                

        public float TotalRemainingHours { get; set; }

        public int TimeTrackCount { get; set; }
    }
}