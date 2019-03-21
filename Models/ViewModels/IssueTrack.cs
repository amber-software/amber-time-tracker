using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Models
{
    public class IssueTrack
    {
        public int IssueID {get; set; }
        
        public string IssueNumber { get; set; }

        public string IssueDescription { get; set; }

        public Platform? IssuePlatform { get; set; }
        
        public float Estimate { get; set; }                

        public float RemainingTime { get; set; }

        public TaskPriority Priority { get; set; }

        public Status Status { get; set; }

        public IList<TimeTrackLogTime> LoggedTimes { get; set; }        
    }

    public class TimeTrackLogTime
    {
        public DateTime TrackDate;

        public int? TimeTrackID { get; set; }

        public float? Hours { get; set; }
    }
}