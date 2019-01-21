using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASR.Models
{
    public class Slot
    {
        [Column("RoomName")]
        [Required]
        [Display(Name = "Room name")]
        public string RoomID { get; set; }
        
        public virtual Room Room { get; set; }

        [Display(Name = "Start time")]
        public DateTime StartTime { get; set; }

        [Required]
        [RegularExpression(@"^(e|E)\\d{5}$", ErrorMessage = "Invalid staff ID")]
        public string StaffID { get; set; }
        
        public virtual Staff Staff { get; set; }

        [Column("BookedInStudentID")]
        public string StudentID { get; set; } = null;
        
        public virtual Student Student { get; set; }
    }
}
