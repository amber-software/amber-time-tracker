using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TimeTracking.Models;

namespace TimeTracking.Models
{
    public class TimeTrackDataContext : IdentityDbContext
    {
        public TimeTrackDataContext (DbContextOptions<TimeTrackDataContext> options)
            : base(options)
        {
        }

        public DbSet<TimeTracking.Models.TimeTrack> TimeTrack { get; set; }        

        public DbSet<TimeTracking.Models.Issue> Issue { get; set; }
    }
}
