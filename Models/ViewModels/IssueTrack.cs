using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Models
{
    public class IssueTrack
    {        
        public string IssueNumber { get; set; }

        public string IssueDescription { get; set; }
        
        public float Estimate { get; set; }                

        public float RemainingTime { get; set; }                

        public IEnumerable<TimeTrackLogTime> LoggedTimes { get; set; }        
    }

    public class TimeTrackLogTime
    {
        public int? TimeTrackID { get; set; }

        public float? Hours { get; set; }
    }
}