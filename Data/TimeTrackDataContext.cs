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

        public DbSet<TimeTracking.Models.BusinessConstant> BusinessConstant { get; set; }        

        public DbSet<TimeTracking.Models.Sprint> Sprint { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TimeTrack>()
                .HasIndex(c => c.OwnerID);
            modelBuilder.Entity<TimeTrack>()
                .Property(c => c.OwnerID).IsRequired();
        }
    }
}
