using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ASR_Api.Models;

namespace ASR_Api.Data
{
    public class ASRContext : DbContext
    {
        public ASRContext (DbContextOptions<ASRContext> options)
            : base(options)
        {
        }
           
        public DbSet<Room> Room { get; set; }

        public DbSet<Staff> Staff { get; set; }

        public DbSet<Student> Student { get; set; }

        public DbSet<Slot> Slot { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Slot>().HasKey(slot => new { slot.RoomID, slot.StartTime });

        }
        

    }
}
