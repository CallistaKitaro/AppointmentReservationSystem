using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ASR.Models
{
    public class ASRContext : DbContext
    {
        public ASRContext (DbContextOptions<ASRContext> options)
            : base(options)
        {
        }

        public DbSet<ASR.Models.Room> Room { get; set; }

        public DbSet<ASR.Models.Staff> Staff { get; set; }

        public DbSet<ASR.Models.Admin> Admin { get; set; }

        public DbSet<ASR.Models.Student> Student { get; set; }

        public DbSet<ASR.Models.Slot> Slot { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Slot>().HasKey(slot => new { slot.RoomID, slot.StartTime });
        }


    }
}
